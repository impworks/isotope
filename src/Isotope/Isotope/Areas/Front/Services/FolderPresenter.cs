using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Impworks.Utils.Linq;
using Impworks.Utils.Strings;
using Isotope.Areas.Front.Dto;
using Isotope.Code.Utils.Date;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Front.Services
{
    /// <summary>
    /// Service class for displaying folders and media to the user.
    /// </summary>
    public class FolderPresenter
    {
        public FolderPresenter(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        #region Tree
        
        /// <summary>
        /// Returns the tree of folders available for the current user.
        /// </summary>
        public async Task<FolderVM[]> GetFolderTreeAsync(UserContext ctx)
        {
            var query = _db.Folders
                           .AsNoTracking()
                           .OrderBy(x => x.Caption)
                           .AsQueryable();
            
            if (ctx.Link != null)
            {
                if(!ctx.Link.IncludeSubfolders)
                    return new FolderVM[0];

                var folder = ctx.Link.Folder;
                query = query.Where(x => x.Path.StartsWith(folder.Path) && x.Depth > folder.Depth); // current folder is root, only include nested ones
            }

            var folders = await query.ToListAsync();
            return BuildFolderTree(folders);
        }
        
        /// <summary>
        /// Combines the folders into a tree.
        /// </summary>
        private FolderVM[] BuildFolderTree(List<Folder> source)
        {
            if(source.Count == 0)
                return new FolderVM[0];
            
            var rootDepth = source.Min(x => x.Depth);
            return source.Where(x => x.Depth == rootDepth).Select(ProcessFolder).ToArray();

            FolderVM ProcessFolder(Folder folder)
            {
                var vm = _mapper.Map<FolderVM>(folder);
                
                var nested = source.Where(x => x.Path.StartsWith(folder.Path) && x.Depth == folder.Depth + 1).ToList();
                foreach (var n in nested)
                    source.Remove(n);
                
                vm.Subfolders = nested.Select(ProcessFolder).ToArray();
                
                return vm;
            }
        }
        
        #endregion

        #region Folder contents

        /// <summary>
        /// Returns the contents of a folder.
        /// </summary>
        public async Task<FolderContentsVM> GetFolderContentsAsync(FolderContentsRequestVM request, UserContext ctx)
        {
            var path = PathHelper.Combine(ctx.Link?.Folder.Path, request.Folder);
            
            var folder = await _db.Folders
                                  .AsNoTracking()
                                  .Include(x => x.Tags)
                                  .GetAsync(x => x.Path == path, $"Folder ({request.Folder})");

            var subfolders = ctx.Link?.IncludeSubfolders != false
                ? await _db.Folders
                           .AsNoTracking()
                           .Where(x => x.Path.StartsWith(path) && x.Depth == folder.Depth + 1)
                           .OrderBy(x => x.Caption)
                           .ProjectToType<FolderVM>(_mapper.Config)
                           .ToArrayAsync()
                : new FolderVM[0];

            var media = await GetMediaByRequest(folder, request, ctx);

            return new FolderContentsVM
            {
                Subfolders = subfolders,
                Tags = _mapper.Map<TagBindingVM[]>(folder.Tags),
                Media = _mapper.Map<MediaThumbnailVM[]>(media)
            };
        }

        /// <summary>
        /// Filters media that matches the requested filter. 
        /// </summary>
        private async Task<IReadOnlyList<Media>> GetMediaByRequest(Folder folder, FolderContentsRequestVM request, UserContext ctx)
        {
            var query = _db.Media
                           .AsNoTracking()
                           .Where(x => x.Folder.Key == folder.Key);

            var tagIds = ctx.Link != null
                ? ctx.Link.Tags.TryParseList<int>(",")
                : request.Tags;

            if (tagIds?.Count > 0)
                query = query.Where(x => x.Tags.Any(y => tagIds.Contains(y.Tag.Id)));

            var media = await query.OrderBy(x => x.Order).ToListAsync();

            var dateFrom = FuzzyDate.TryParse(StringHelper.Coalesce(request.DateFrom, ctx.Link?.DateFrom));
            var dateTo = FuzzyDate.TryParse(StringHelper.Coalesce(request.DateTo, ctx.Link?.DateTo));

            if (dateTo != null || dateFrom != null)
            {
                media = media.Select(x => new
                             {
                                 Date = FuzzyDate.TryParse(x.Date),
                                 Media = x
                             })
                             .Where(x => x.Date != null)
                             .Where(x => (dateFrom == null || x.Date >= dateFrom) && (dateTo == null || x.Date <= dateTo))
                             .Select(x => x.Media)
                             .ToList();
            }

            return media;
        }

        #endregion
    }
}