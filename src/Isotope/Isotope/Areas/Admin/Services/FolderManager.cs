using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace Isotope.Areas.Admin.Services
{
    /// <summary>
    /// Service for managing the folder tree.
    /// </summary>
    public class FolderManager
    {
        public FolderManager(AppDbContext db, IMapper mapper, IBackgroundJobService jobSvc)
        {
            _db = db;
            _mapper = mapper;
            _jobSvc = jobSvc;
        }
        
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IBackgroundJobService _jobSvc;

        /// <summary>
        /// Returns the entire folder tree for display.
        /// </summary>
        public async Task<IReadOnlyList<FolderTitleVM>> GetTreeAsync()
        {
            var folders = await _db.Folders
                                   .AsNoTracking()
                                   .Include(x => x.Thumbnail)
                                   .Where(x => x.Depth > 0)
                                   .OrderBy(x => x.Caption)
                                   .ToListAsync();

            return folders.Where(x => x.Depth == 1).ToList().Select(ProcessFolder).ToList();
            
            FolderTitleVM ProcessFolder(Folder folder)
            {
                var vm = _mapper.Map<FolderTitleVM>(folder);
                
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

            var folder = _mapper.Map<Folder>(vm);
            folder.Key = UniqueKey.Get();
            folder.Depth = parent.Depth + 1;
            folder.Path = PathHelper.Combine(parent.Path, folder.Slug);
            folder.Tags = vm.Tags?.Select(x => new FolderTagBinding {TagId = x}).ToList();

            _db.Folders.Add(folder);
            
            await _db.SaveChangesAsync();

            return _mapper.Map<FolderTitleVM>(folder);
        }

        /// <summary>
        /// Returns the details of a folder.
        /// </summary>
        public async Task<FolderVM> GetAsync(string key)
        {
            var folder = await _db.Folders
                                  .AsNoTracking()
                                  .Include(x => x.Tags)
                                  .FirstOrDefaultAsync(x => x.Key == key);
            
            if(folder == null)
                throw new OperationException($"Folder '{key}' does not exist.");

            return _mapper.Map<FolderVM>(folder);
        }

        /// <summary>
        /// Removes the folder with all subfolders and media inside it.
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            var folder = await _db.Folders
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(x => x.Key == key);
            
            if(folder == null)
                throw new OperationException($"Folder '{key}' does not exist.");

            var folders = await _db.Folders
                                   .Where(x => x.Path == folder.Path || x.Path.StartsWith(folder.Path + "/"))
                                   .ToListAsync();
            
            _db.Folders.RemoveRange(folders);
            await _db.SaveChangesAsync();
            
            await _db.Media.RemoveWhereAsync(x => x.Folder.Path == folder.Path || x.Folder.Path.StartsWith(folder.Path + "/"));
            await _db.SaveChangesAsync();

            foreach (var f in folders)
            {
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "@media", f.Key);
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
            
            var folder = await _db.Folders
                                  .Include(x => x.Tags)
                                  .FirstOrDefaultAsync(x => x.Key == key);
            
            if (folder.Slug != vm.Slug)
            {
                // update paths in subfolders
                var oldPath = folder.Path;
                var newPath = PathHelper.Combine(PathHelper.GetParentPath(folder.Path), vm.Slug);

                folder.Path = newPath;
                
                var subfolders = await _db.Folders
                                          .Where(x => x.Path.StartsWith(oldPath) && x.Depth > folder.Depth)
                                          .ToListAsync();
                
                foreach (var subfolder in subfolders)
                    subfolder.Path = newPath + subfolder.Path.Substring(oldPath.Length);
            }

            await UpdateInheritedTagsAsync(folder, vm.Tags);

            _mapper.Map(vm, folder);
            folder.Tags = vm.Tags?.Select(x => new FolderTagBinding {TagId = x}).ToList();

            await _db.SaveChangesAsync();

            return _mapper.Map<FolderTitleVM>(folder);
        }

        /// <summary>
        /// Moves the folder to be a child of another folder.
        /// </summary>
        public async Task MoveAsync(string folderKey, string targetKey)
        {
            var folder = await _db.Folders.AsNoTracking().FirstOrDefaultAsync(x => x.Key == folderKey);
            if (folder == null)
                throw new OperationException($"Folder '{folderKey}' does not exist.");

            var target = string.IsNullOrEmpty(targetKey)
                ? await _db.Folders.AsNoTracking().FirstOrDefaultAsync(x => x.Depth == 0)
                : await _db.Folders.AsNoTracking().FirstOrDefaultAsync(x => x.Key == targetKey);
            
            if (target == null)
                throw new OperationException($"Folder '{targetKey}' does not exist.");

            if(target.Path.StartsWith(folder.Path))
                throw new OperationException("Folder cannot be moved inside itself.");

            var oldPath = folder.Path;
            var newPath = PathHelper.Combine(target.Path, folder.Slug);
            var depthDiff = target.Depth - folder.Depth + 1;

            var allFolders = await _db.Folders
                                      .Where(x => x.Key == folderKey || x.Path.StartsWith(oldPath + "/"))
                                      .ToListAsync();
            
            var newPaths = allFolders.Select(x => x.Path.Replace(oldPath, newPath)).ToList();
            var conflict = await _db.Folders.FirstOrDefaultAsync(x => newPaths.Contains(x.Path));
            if (conflict != null)
                throw new OperationException($"Folder name conflict: path '{conflict.Path}' already exists at destination.");

            foreach (var f in allFolders)
            {
                f.Path = f.Path.Replace(oldPath, newPath);
                f.Depth += depthDiff;
            }

            await _db.SaveChangesAsync();
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
            if (!Regex.IsMatch(vm.Slug, "^[a-z0-9-]+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                throw new OperationException("Slug contains invalid characters. Only English letters, numbers and a hyphen are allowed.");
                
            if (!string.IsNullOrEmpty(vm.ThumbnailKey))
            {
                var exists = await _db.Media.AnyAsync(x => x.Key == vm.ThumbnailKey);
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
                var tags = await _db.Tags
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
                    return await _db.Folders
                                    .AsNoTracking()
                                    .Where(x => x.Path.StartsWith(parent.Path) && x.Depth == parent.Depth + 1)
                                    .ToListAsync();
                }
                else
                {
                    var folder = await _db.Folders
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(x => x.Key == key);
                    if(folder == null)
                        throw new OperationException($"Folder '{key}' does not exist.");

                    var parentPath = PathHelper.GetParentPath(folder.Path);
                    return await _db.Folders
                                    .AsNoTracking()
                                    .Where(x => x.Path.StartsWith(parentPath)
                                        && x.Depth == folder.Depth
                                        && x.Key != key)
                                    .ToListAsync();
                }
            }
        }

        /// <summary>
        /// Returns the parent folder by its key.
        /// </summary>
        private async Task<Folder> GetParentFolderAsync(string key)
        {
            var q = _db.Folders.AsNoTracking();
            var folder = string.IsNullOrEmpty(key)
                ? await q.FirstOrDefaultAsync(x => x.Depth == 0)
                : await q.FirstOrDefaultAsync(x => x.Key == key);

            if (folder == null)
                throw new OperationException($"Folder '{StringHelper.Coalesce(key, "root")}' does not exist!");

            return folder;
        }

        /// <summary>
        /// Recalculates all inherited tags for all media inside the folder and subfolders.
        /// </summary>
        private async Task UpdateInheritedTagsAsync(Folder folder, int[] tags)
        {
            var oldTags = folder.Tags?.Select(x => x.TagId).OrderBy(x => x).ToList() ?? new List<int>();
            var newTags = tags?.OrderBy(x => x).ToList() ?? new List<int>();
            
            if (oldTags.SequenceEqual(newTags))
                return;

            await _jobSvc.RunAsync(JobBuilder.For<RebuildInheritedTagsJob>().SupersedeAll());
        }
    }
}