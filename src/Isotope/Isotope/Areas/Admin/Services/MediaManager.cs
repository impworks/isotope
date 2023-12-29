using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Impworks.Utils.Format;
using Impworks.Utils.Linq;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services.Jobs;
using Isotope.Areas.Admin.Services.MediaHandlers;
using Isotope.Areas.Admin.Utils;
using Isotope.Code.Services.Jobs;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Admin.Services;

/// <summary>
/// Service for managing the media files.
/// </summary>
public class MediaManager
{
    public MediaManager(AppDbContext db, IMapper mapper, IEnumerable<IMediaHandler> mediaHandlers, IBackgroundJobService jobSvc)
    {
        _db = db;
        _mapper = mapper;
        _jobSvc = jobSvc;
        _mediaHandlers = mediaHandlers.ToArray();
    }
        
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IBackgroundJobService _jobSvc;
    private readonly IMediaHandler[] _mediaHandlers;

    /// <summary>
    /// Returns the list of all media files in a folder.
    /// </summary>
    public async Task<IReadOnlyList<MediaThumbnailVM>> GetListAsync(string folderKey)
    {
        if(!await _db.Folders.AnyAsync(x => x.Key == folderKey))
            throw new OperationException($"Folder '{folderKey}' does not exist.");

        return await _db.Media
                        .Where(x => x.FolderKey == folderKey)
                        .OrderBy(x => x.Order)
                        .ProjectToType<MediaThumbnailVM>(_mapper.Config)
                        .ToListAsync();
    }

    /// <summary>
    /// Adds a new file from uploaded data.
    /// </summary>
    public async Task<MediaThumbnailVM> UploadAsync(string folderKey, IFormFile file)
    {
        var folder = await _db.Folders.GetAsync(x => x.Key == folderKey, $"Folder '{folderKey}' does not exist.");
        var handler = _mediaHandlers.FirstOrDefault(x => x.SupportedMimeTypes.Contains(file.ContentType));
        if(handler == null)
            throw new OperationException($"Media type '{file.ContentType}' is not supported!");

        var key = UniqueKey.Get();
        var paths = await SaveUploadAsync(file, folder, key);
        using var mediaInfo = await handler.ProcessAsync(key, paths.Path);

        var tagsIds = await GetInheritedTagsAsync(folder);
        var tags = tagsIds.Select(x => new MediaTagBinding {TagId = x, Type = TagBindingType.Inherited}).ToList();
            
        var maxOrder = await _db.Media
                                .Where(x => x.FolderKey == folderKey)
                                .MaxAsync(x => (int?) x.Order) ?? 0;

        var media = new Media
        {
            Key = key,
            FolderKey = folder.Key,
            Path = paths.Url,
            Type = mediaInfo.MediaType,
            IsReady = mediaInfo.IsReady,
            Date = mediaInfo.Date?.ToString("yyyy.MM.dd"),
            Width = mediaInfo.FullImage.Width,
            Height = mediaInfo.FullImage.Height,
            ThumbnailRect = ImageHelper.GetDefaultThumbnailRect(mediaInfo.FullImage.Size),
            UploadDate = DateTime.Now,
            VersionDate = DateTime.Now,
            Order = maxOrder + 1,
            Tags = tags
        };

        _db.Media.Add(media);
        await _db.SaveChangesAsync();

        folder.ThumbnailKey ??= media.Key;
        folder.MediaCount = await _db.Media.CountAsync(x => x.FolderKey == folderKey);
        await _db.SaveChangesAsync();

        await ImageHelper.CreateThumbnailsAsync(mediaInfo.FullImage, paths.Path);

        return await _db.Media
                        .Where(x => x.Key == key)
                        .ProjectToType<MediaThumbnailVM>(_mapper.Config)
                        .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Returns media details by key.
    /// </summary>
    public async Task<MediaVM> GetAsync(string key)
    {
        var media = await _db.Media
                             .Include(x => x.Tags)
                             .AsNoTracking()
                             .GetAsync(x => x.Key == key, $"Media '{key}' does not exist.");

        return _mapper.Map<MediaVM>(media);
    }

    /// <summary>
    /// Updates the media description.
    /// </summary>
    public async Task UpdateAsync(string key, MediaVM vm)
    {
        Validate(vm);
            
        var media = await _db.Media
                             .Include(x => x.Tags)
                             .GetAsync(x => x.Key == key, $"Media '{key}' does not exist.");
            
        _mapper.Map(vm, media);

        var newTagIds = (vm.OverlayTags?.Select(x => x.TagId) ?? Array.Empty<int>())
                        .Concat(vm.ExtraTags?.Select(x => x.TagId) ?? Array.Empty<int>())
                        .ToList();
            
        var existingTagIds = await _db.Tags
                                      .Where(x => newTagIds.Contains(x.Id))
                                      .Select(x => x.Id)
                                      .ToListAsync();

        var missingTagIds = newTagIds.Except(existingTagIds).ToList();
        if(missingTagIds.Any())
            throw new OperationException($"Tag(s) {missingTagIds.JoinString(", ")} do not exist.");
            
        var newTags = new List<MediaTagBinding>();
            
        if(vm.OverlayTags != null)
            newTags.AddRange(_mapper.Map<MediaTagBinding[]>(vm.OverlayTags));
            
        if(vm.ExtraTags != null)
            newTags.AddRange(_mapper.Map<MediaTagBinding[]>(vm.ExtraTags));

        foreach (var tag in newTags)
            tag.Media = media;
            
        _db.MediaTags.RemoveRange(media.Tags.Where(x => x.Type != TagBindingType.Inherited));
        _db.MediaTags.AddRange(newTags);

        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Returns the thumbnail locations of a photo.
    /// </summary>
    public async Task<RectVM> GetThumbnailAsync(string key)
    {
        var media = await _db.Media
                             .Include(x => x.Tags)
                             .GetAsync(x => x.Key == key, $"Media '{key}' does not exist.");

        return _mapper.Map<RectVM>(media.ThumbnailRect);
    }

    /// <summary>
    /// Updates the square rectangle of the media.
    /// </summary>
    public async Task UpdateThumbnailAsync(string key, RectVM vm)
    {
        Validate(vm, "thumbnail rectangle");
            
        var media = await _db.Media
                             .Include(x => x.Tags)
                             .GetAsync(x => x.Key == key, $"Media '{key}' does not exist.");

        var rect = _mapper.Map<Rect>(vm);
        if (media.ThumbnailRect.Equals(rect))
            return;

        media.ThumbnailRect = rect; 
        await _db.SaveChangesAsync();

        await _jobSvc.RunAsync(JobBuilder.For<UpdateThumbnailJob>().WithArgs(media.Key));
    }

    /// <summary>
    /// Updates the order of all media files in a folder.
    /// </summary>
    public async Task ReorderAsync(string folderKey, IReadOnlyList<string> mediaKeys)
    {
        var folder = await _db.Folders.GetAsync(x => x.Key == folderKey, $"Folder '{folderKey}' does not exist.");

        var extraCount = mediaKeys.Count;
        var orderLookup = new Dictionary<string, int>();
        for (var i = 0; i < mediaKeys.Count; i++)
            orderLookup[mediaKeys[i]] = i;

        var media = await _db.Media
                             .Where(x => x.FolderKey == folderKey)
                             .OrderBy(x => x.UploadDate)
                             .ToListAsync();

        foreach (var elem in media)
        {
            elem.Order = orderLookup.TryGetValue(elem.Key, out var order)
                ? order
                : extraCount++;
        }

        folder.MediaCount = media.Count;
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Removes all media files specified by the list of keys.
    /// Throws an exception if any of the keys is missing.
    /// </summary>
    public async Task RemoveAsync(IReadOnlyList<string> keys)
    {
        var keyBatches = keys.PartitionBySize(100);
        var paths = new List<string>(keys.Count);
            
        foreach (var keyBatch in keyBatches)
        {
            var media = await _db.Media
                                 .Include(x => x.Folder)
                                 .Where(x => keyBatch.Contains(x.Key))
                                 .ToListAsync();

            var missingKey = keyBatch.Except(media.Select(x => x.Key)).FirstOrDefault();
            if(missingKey != null)
                throw new OperationException($"Media '{missingKey}' does not exist.");

            foreach (var m in media)
            {
                if (m.Folder.ThumbnailKey == m.Key)
                    m.Folder.Thumbnail = null;

                m.Folder.MediaCount--;

                _db.Media.Remove(m);
                paths.Add(m.Path);
            }
        }

        await _db.SaveChangesAsync();

        var sizedPaths = from p in paths
                         from s in EnumHelper.GetEnumValues<MediaSize>()
                         let sizedPath = MediaHelper.GetSizedMediaPath(p, s)
                         where File.Exists(sizedPath)
                         select sizedPath;
            
        foreach (var sizedPath in sizedPaths)
            File.Delete(sizedPath);
    }

    /// <summary>
    /// Moves all media files specified by keys to another folder.
    /// </summary>
    public async Task MoveAsync(string folderKey, IReadOnlyList<string> keys)
    {
        var folder = await _db.Folders
                              .FirstOrDefaultAsync(x => x.Key == folderKey)
            ?? throw new OperationException($"Folder '{folderKey}' does not exist.");
            
        var keyBatches = keys.PartitionBySize(100);
        foreach (var keyBatch in keyBatches)
        {
            var media = await _db.Media
                                 .Include(x => x.Folder)
                                 .Where(x => keyBatch.Contains(x.Key))
                                 .ToListAsync();

            var missingKey = keyBatch.Except(media.Select(x => x.Key)).FirstOrDefault();
            if(missingKey != null)
                throw new OperationException($"Media '{missingKey}' does not exist.");

            foreach (var m in media)
            {
                if(m.FolderKey == folderKey)
                    throw new OperationException($"Media '{m.Key}' is already in target folder ({folder.Caption}).");
                            
                if (m.Folder.ThumbnailKey == m.Key)
                    m.Folder.Thumbnail = null;

                m.Folder.MediaCount--;
                folder.MediaCount++;
                    
                m.FolderKey = folderKey;
            }
        }

        await _db.SaveChangesAsync();
    }

    /**
     * Updates a list of media files, changing their description and date.
     */
    public async Task UpdateAsync(MassMediaUpdateVM vm)
    {
        if(vm == null)
            throw new OperationException("Incorrect request.");
            
        if(vm.Date?.IsSet != true && vm.Description?.IsSet != true)
            throw new OperationException("No properties are set for update.");
            
        if((vm.Keys?.Length >= 1) == false)
            throw new OperationException("No media files are set for update.");
            
        var keyBatches = vm.Keys.PartitionBySize(100);
        foreach (var keyBatch in keyBatches)
        {
            var media = await _db.Media
                                 .Where(x => keyBatch.Contains(x.Key))
                                 .ToListAsync();

            var missingKey = keyBatch.Except(media.Select(x => x.Key)).FirstOrDefault();
            if(missingKey != null)
                throw new OperationException($"Media '{missingKey}' does not exist.");

            foreach (var m in media)
            {
                if (vm.Date?.IsSet == true)
                    m.Date = vm.Date.Value;

                if (vm.Description?.IsSet == true)
                    m.Description = vm.Description.Value;
            }
        }

        await _db.SaveChangesAsync();
    }

    #region Helpers

    /// <summary>
    /// Caches the uploaded file to disk within specified folder.
    /// </summary>
    private async Task<(string Path, string Url)> SaveUploadAsync(IFormFile file, Folder folder, string key)
    {
        var fileName = key + Path.GetExtension(file.FileName);
        var path = $"/@media/{folder.Key}/{fileName}";
        var localPath = MediaHelper.GetFullMediaPath(path);
            
        Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
            
        await using var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write);
        await file.CopyToAsync(fs);

        return (localPath, path);
    }

    /// <summary>
    /// Returns tag IDs inherited from all parent folders, including this one. 
    /// </summary>
    private async Task<IReadOnlyList<int>> GetInheritedTagsAsync(Folder folder)
    {
        var paths = PathHelper.GetParentPaths(folder.Path);
        return await _db.Folders
                        .Where(x => paths.Contains(x.Path))
                        .SelectMany(x => x.Tags)
                        .Select(x => x.TagId)
                        .Distinct()
                        .ToListAsync();
    }

    #endregion

    #region Validation

    /// <summary>
    /// Validates the media details.
    /// </summary>
    private void Validate(MediaVM vm)
    {
        if (vm.OverlayTags != null)
            foreach (var t in vm.OverlayTags)
                Validate(t.Location, $"tag '{t.TagId}'");
    }

    /// <summary>
    /// Validates a rectangle.
    /// </summary>
    private void Validate(RectVM vm, string descr)
    {
        if(vm == null)
            throw new OperationException("Rect is not specified for " + descr);

        vm.X = Clamp(vm.X);
        vm.Y = Clamp(vm.Y);
        vm.Width = Clamp(vm.Width);
        vm.Height = Clamp(vm.Height);

        if (vm.X + vm.Width > 1)
            vm.Width = Clamp(1 - vm.X);

        if (vm.Y + vm.Height > 1)
            vm.Height = Clamp(1 - vm.Y);

        double Clamp(double value)
        {
            return value switch
            {
                < 0 => 0,
                > 1 => 1,
                _ => value
            };
        }
    }
        
    #endregion
}