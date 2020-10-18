using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Impworks.Utils.Format;
using Impworks.Utils.Linq;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Utils;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Admin.Services
{
    /// <summary>
    /// Service for managing the media files.
    /// </summary>
    public class MediaManager
    {
        public MediaManager(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        /// <summary>
        /// Returns the list of media files.
        /// </summary>
        public async Task<IReadOnlyList<MediaThumbnailVM>> GetListAsync(MediaListRequestVM vm)
        {
            const int PAGE_SIZE = 100;
            
            var query = _db.Media
                           .AsNoTracking()
                           .OrderBy(vm.OrderBy.TryGetOneOf(nameof(Media.Order), nameof(Media.UploadDate), nameof(Media.Date)), vm.OrderDesc)
                           .AsQueryable();

            if (!string.IsNullOrEmpty(vm.Folder))
            {
                if(! await _db.Folders.AnyAsync(x => x.Key == vm.Folder))
                    throw new OperationException($"Folder '{vm.Folder}' does not exist.");
                
                query = query.Where(x => x.FolderKey == vm.Folder);
            }

            return await query.Skip(Math.Max(0, vm.Page) * PAGE_SIZE)
                              .Take(PAGE_SIZE)
                              .ProjectToType<MediaThumbnailVM>(_mapper.Config)
                              .ToListAsync();
        }

        /// <summary>
        /// Returns media details by key.
        /// </summary>
        public async Task<MediaVM> GetAsync(string key)
        {
            var media = await _db.Media
                                 .Include(x => x.Tags)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(x => x.Key == key);
            
            if(media == null)
                throw new OperationException($"Media '{key}' does not exist.");

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
                                 .FirstOrDefaultAsync(x => x.Key == key);
            
            if(media == null)
                throw new OperationException($"Media '{key}' does not exist.");

            _mapper.Map(vm, media);

            var newTagIds = (vm.OverlayTags?.Select(x => x.TagId) ?? new int[0])
                            .Concat(vm.ExtraTags?.Select(x => x.TagId) ?? new int[0])
                            .OrderBy(x => x)
                            .ToList();

            var oldTagIds = media.Tags
                                 .Where(x => x.Type != TagBindingType.Inherited).Select(x => x.TagId)
                                 .OrderBy(x => x);

            if (!oldTagIds.SequenceEqual(newTagIds))
            {
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
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the square rectangle of the media.
        /// </summary>
        public async Task UpdateThumbnailAsync(string key, RectVM vm)
        {
            Validate(vm, "thumbnail rectangle");
            
            var media = await _db.Media
                                 .Include(x => x.Tags)
                                 .FirstOrDefaultAsync(x => x.Key == key);
            
            if(media == null)
                throw new OperationException($"Media '{key}' does not exist.");

            media.ThumbnailRect = _mapper.Map<Rect>(vm);
            
            // todo: launch image regeneration job

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the specified media by key.
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            var media = await _db.Media.FirstOrDefaultAsync(x => x.Key == key);
            if(media == null)
                throw new OperationException($"Media '{key}' does not exist.");

            _db.Media.Remove(media);

            await _db.SaveChangesAsync();

            foreach (var size in EnumHelper.GetEnumValues<MediaSize>())
            {
                var path = MediaHelper.GetSizedMediaPath(media.Path, size);
                if(File.Exists(path))
                    File.Delete(path);
            }
        }

        /// <summary>
        /// Validates the media details.
        /// </summary>
        /// <param name="vm"></param>
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
            
            if(vm.X < 0 || vm.X > 1)
                throw new OperationException($"X coordinate must be in 0..1 range for {descr}.");
            
            if(vm.Y < 0 || vm.Y > 1)
                throw new OperationException($"Y coordinate must be in 0..1 range for {descr}.");
            
            if(vm.Width < 0 || vm.Width > 1)
                throw new OperationException($"Width must be in 0..1 range for {descr}.");
            
            if(vm.Height < 0 || vm.Height > 1)
                throw new OperationException($"Height must be in 0..1 range for {descr}.");
            
            if(vm.X + vm.Width > 1)
                throw new OperationException($"Width must not exceed media size for {descr}.");
            
            if(vm.Y + vm.Height > 1)
                throw new OperationException($"Width must not exceed media size for {descr}.");
        }
    }
}