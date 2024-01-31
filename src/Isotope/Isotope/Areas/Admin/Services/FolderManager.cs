using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Impworks.Utils.Linq;
using Impworks.Utils.Strings;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services.Jobs;
using Isotope.Areas.Admin.Utils;
using Isotope.Code.Services.Jobs;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Admin.Services;

/// <summary>
/// Service for managing the folder tree.
/// </summary>
public partial class FolderManager(AppDbContext db, IMapper mapper, IBackgroundJobService jobSvc)
{
    /// <summary>
    /// Returns the entire folder tree for display.
    /// </summary>
    public async Task<IReadOnlyList<FolderTitleVM>> GetTreeAsync(string rootKey = null)
    {
        var foldersQuery = db.Folders
                             .AsNoTracking()
                             .Include(x => x.Thumbnail)
                             .AsQueryable();

        if (string.IsNullOrEmpty(rootKey))
        {
            foldersQuery = foldersQuery.Where(x => x.Depth > 0);
        }
        else
        {
            var root = await db.Folders
                               .AsNoTracking()
                               .GetAsync(x => x.Key == rootKey, $"Folder '{rootKey}' does not exist.");

            foldersQuery = foldersQuery.Where(x => x.Path.StartsWith(root.Path) && x.Depth > root.Depth);
        }

        var folders = await foldersQuery.OrderBy(x => x.Caption)
                                        .ToListAsync();

        var minDepth = folders.Min(x => x.Depth);
        return folders.Where(x => x.Depth == minDepth).ToList().Select(ProcessFolder).ToList();
            
        FolderTitleVM ProcessFolder(Folder folder)
        {
            var vm = mapper.Map<FolderTitleVM>(folder);
                
            var nested = folders.Where(x => x.Path.StartsWith(folder.Path) && x.Depth == folder.Depth + 1).ToList();
            foreach (var n in nested)
                folders.Remove(n);
                
            vm.Subfolders = nested.Select(ProcessFolder).ToArray();
                
            return vm;
        }
    }

    /// <summary>
    /// Adds a new folder.
    /// </summary>
    public async Task<FolderTitleVM> CreateAsync(string parentKey, FolderVM vm)
    {
        await ValidateAsync(vm, null, parentKey);

        var parent = await GetParentFolderAsync(parentKey);

        var folder = mapper.Map<Folder>(vm);
        folder.Key = UniqueKey.Get();
        folder.Depth = parent.Depth + 1;
        folder.Path = PathHelper.Combine(parent.Path, folder.Slug);
        folder.Tags = vm.Tags?.Select(x => new FolderTagBinding {TagId = x}).ToList();

        db.Folders.Add(folder);
            
        await db.SaveChangesAsync();

        return mapper.Map<FolderTitleVM>(folder);
    }

    /// <summary>
    /// Returns the details of a folder.
    /// </summary>
    public async Task<FolderVM> GetAsync(string key)
    {
        var folder = await db.Folders
                              .AsNoTracking()
                              .Include(x => x.Tags)
                              .FirstOrDefaultAsync(x => x.Key == key);
            
        if(folder == null)
            throw new OperationException($"Folder '{key}' does not exist.");

        return mapper.Map<FolderVM>(folder);
    }

    /// <summary>
    /// Removes the folder with all subfolders and media inside it.
    /// </summary>
    public async Task RemoveAsync(string key)
    {
        var folder = await db.Folders
                              .AsNoTracking()
                              .GetAsync(x => x.Key == key, $"Folder '{key}' does not exist.");

        var folders = await db.Folders
                              .Where(x => x.Path == folder.Path || x.Path.StartsWith(folder.Path + "/"))
                              .ToListAsync();

        await db.SharedLinks.RemoveWhereAsync(x => x.Folder.Path == folder.Path || x.Folder.Path.StartsWith(folder.Path + "/"));
        await db.SaveChangesAsync();

        db.Folders.RemoveRange(folders);
        await db.SaveChangesAsync();
            
        await db.Media.RemoveWhereAsync(x => x.Folder.Path == folder.Path || x.Folder.Path.StartsWith(folder.Path + "/"));
        await db.SaveChangesAsync();

        foreach (var f in folders)
        {
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "@media", f.Key);
            if(Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }

    /// <summary>
    /// Updates the existing folder.
    /// </summary>
    public async Task<FolderTitleVM> UpdateAsync(string key, FolderVM vm)
    {
        await ValidateAsync(vm, key);
            
        var folder = await db.Folders
                              .Include(x => x.Tags)
                              .GetAsync(x => x.Key == key, $"Folder {key} not found");

        if (folder.Slug != vm.Slug)
        {
            // update paths in subfolders
            var oldPath = folder.Path;
            var newPath = PathHelper.Combine(PathHelper.GetParentPath(folder.Path), vm.Slug);

            folder.Path = newPath;
                
            var subfolders = await db.Folders
                                      .Where(x => x.Path.StartsWith(oldPath) && x.Depth > folder.Depth)
                                      .ToListAsync();
                
            foreach (var subfolder in subfolders)
                subfolder.Path = newPath + subfolder.Path.Substring(oldPath.Length);
        }

        mapper.Map(vm, folder);
            
        var oldTags = folder.Tags?.Select(x => x.TagId).OrderBy(x => x).ToList() ?? new List<int>();
        var newTags = vm.Tags?.OrderBy(x => x).ToList() ?? new List<int>();
            
        folder.Tags = vm.Tags?.Select(x => new FolderTagBinding {TagId = x}).ToList();

        await db.SaveChangesAsync();
            
        if(!oldTags.SequenceEqual(newTags))
            await jobSvc.RunAsync(JobBuilder.For<RebuildInheritedTagsJob>().SupersedeAll()); // sic! run in background

        return mapper.Map<FolderTitleVM>(folder);
    }

    /// <summary>
    /// Moves the folder to be a child of another folder.
    /// </summary>
    public async Task MoveAsync(string folderKey, string targetKey)
    {
        var folder = await db.Folders.AsNoTracking().FirstOrDefaultAsync(x => x.Key == folderKey);
        if (folder == null)
            throw new OperationException($"Folder '{folderKey}' does not exist.");

        var target = string.IsNullOrEmpty(targetKey)
            ? await db.Folders.AsNoTracking().FirstOrDefaultAsync(x => x.Depth == 0)
            : await db.Folders.AsNoTracking().FirstOrDefaultAsync(x => x.Key == targetKey);
            
        if (target == null)
            throw new OperationException($"Folder '{targetKey}' does not exist.");

        if(target.Path.StartsWith(folder.Path))
            throw new OperationException("Folder cannot be moved inside itself.");

        var oldPath = folder.Path;
        var newPath = PathHelper.Combine(target.Path, folder.Slug);
        var depthDiff = target.Depth - folder.Depth + 1;

        var allFolders = await db.Folders
                                  .Where(x => x.Key == folderKey || x.Path.StartsWith(oldPath + "/"))
                                  .ToListAsync();
            
        var newPaths = allFolders.Select(x => x.Path.Replace(oldPath, newPath)).ToList();
        var conflict = await db.Folders.FirstOrDefaultAsync(x => newPaths.Contains(x.Path));
        if (conflict != null)
            throw new OperationException($"Folder name conflict: path '{conflict.Path}' already exists at destination.");

        foreach (var f in allFolders)
        {
            f.Path = f.Path.Replace(oldPath, newPath);
            f.Depth += depthDiff;
        }

        await db.SaveChangesAsync();
            
        await jobSvc.RunAsync(JobBuilder.For<RebuildInheritedTagsJob>().SupersedeAll());
    }

    /// <summary>
    /// Ensures that the folder details are valid.
    /// </summary>
    private async Task ValidateAsync(FolderVM vm, string key, string parentKey = null)
    {
        if(string.IsNullOrEmpty(vm.Caption))
            throw new OperationException("Caption is required.");
            
        if(string.IsNullOrEmpty(vm.Slug))
            throw new OperationException("Slug is required.");
        if (!ValidIdentifierRegex().IsMatch(vm.Slug))
            throw new OperationException("Slug contains invalid characters. Only English letters, numbers and a hyphen are allowed.");
                
        if (!string.IsNullOrEmpty(vm.ThumbnailKey))
        {
            var exists = await db.Media.AnyAsync(x => x.Key == vm.ThumbnailKey);
            if(!exists)
                throw new OperationException($"Media '{vm.ThumbnailKey}' does not exist.");
        }

        var neighbours = await GetNeighboursAsync();
        foreach (var n in neighbours)
        {
            if(n.Caption.Equals(vm.Caption, StringComparison.InvariantCultureIgnoreCase))
                throw new OperationException($"Folder with caption '{vm.Caption}' already exists nearby.");
                
            if(n.Slug.Equals(vm.Slug, StringComparison.InvariantCultureIgnoreCase))
                throw new OperationException($"Folder with slug '{vm.Slug}' already exists nearby.");
        }
            
        if (vm.Tags?.Length > 0)
        {
            var tags = await db.Tags
                                .Where(x => vm.Tags.Contains(x.Id))
                                .Select(x => x.Id)
                                .ToHashSetAsync();

            var missingTags = vm.Tags.Where(x => !tags.Contains(x)).ToList();
            if(missingTags.Any())
                throw new OperationException($"Tag(s) {missingTags.JoinString(", ")} do not exists.");
        }

        async Task<IReadOnlyList<Folder>> GetNeighboursAsync()
        {
            if (string.IsNullOrEmpty(key))
            {
                var parent = await GetParentFolderAsync(parentKey);
                return await db.Folders
                                .AsNoTracking()
                                .Where(x => x.Path.StartsWith(parent.Path) && x.Depth == parent.Depth + 1)
                                .ToListAsync();
            }

            var folder = await db.Folders
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(x => x.Key == key);
            if(folder == null)
                throw new OperationException($"Folder '{key}' does not exist.");

            var parentPath = PathHelper.GetParentPath(folder.Path);
            return await db.Folders
                            .AsNoTracking()
                            .Where(x => x.Path.StartsWith(parentPath)
                                && x.Depth == folder.Depth
                                && x.Key != key)
                            .ToListAsync();
        }
    }

    /// <summary>
    /// Returns the parent folder by its key.
    /// </summary>
    private async Task<Folder> GetParentFolderAsync(string key)
    {
        var q = db.Folders.AsNoTracking();
        var folder = string.IsNullOrEmpty(key)
            ? await q.FirstOrDefaultAsync(x => x.Depth == 0)
            : await q.FirstOrDefaultAsync(x => x.Key == key);

        if (folder == null)
            throw new OperationException($"Folder '{StringHelper.Coalesce(key, "root")}' does not exist!");

        return folder;
    }

    [GeneratedRegex("^[a-z0-9-]+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ValidIdentifierRegex();
}