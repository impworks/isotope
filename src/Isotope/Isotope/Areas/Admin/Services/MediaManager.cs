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
    }
}