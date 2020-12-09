using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Impworks.Utils.Linq;
using Impworks.Utils.Strings;
using Isotope.Areas.Front.Dto;
using Isotope.Code.Utils.Date;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
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
                           .Include(x => x.Thumbnail)
                           .AsNoTracking()
                           .Where(x => x.Depth > 0)
                           .OrderBy(x => x.Caption)
                           .AsQueryable();

            Folder baseFolder = null;
            
            if (ctx.Link is SharedLink link)
            {
                if(link.Scope == SearchScope.CurrentFolder)
                    return new FolderVM[0];

                baseFolder = link.Folder;
                query = query.Where(x => x.Path.StartsWith(baseFolder.Path) && x.Depth > baseFolder.Depth); // current folder is root, only include nested ones
            }

            var folders = await query.ToListAsync();
            return BuildFolderTree(folders, baseFolder);
        }
        
        /// <summary>
        /// Combines the folders into a tree.
        /// </summary>
        private FolderVM[] BuildFolderTree(List<Folder> source, Folder baseFolder)
        {
            if(source.Count == 0)
                return new FolderVM[0];
            
            var rootDepth = source.Min(x => x.Depth);
            return source.Where(x => x.Depth == rootDepth).ToList().Select(ProcessFolder).ToArray();

            FolderVM ProcessFolder(Folder folder)
            {
                var vm = _mapper.Map<FolderVM>(folder);
                if (baseFolder != null)
                    vm.Path = vm.Path.Replace(baseFolder.Path, "");
                
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
            var req = CombineRequest(request, ctx);
            var hasFilter = req.Tags != null || req.From != null || req.To != null;
            return hasFilter
                ? await GetFolderFilteredContentsAsync(req)
                : await GetFolderSimpleContentsAsync(req, ctx);
        }
        
        /// <summary>
        /// Creates the actual search request from user input and shared link.
        /// </summary>
        private FolderContentsRequestVM CombineRequest(FolderContentsRequestVM request, UserContext ctx)
        {
            if (ctx.Link == null)
            {
                request.Folder = PathHelper.Normalize(request.Folder);
                return request;
            }

            var link = ctx.Link;
            var rootPath = link.Folder.Path;
            var hasFilter = link.Tags != null || link.DateFrom != null || link.DateTo != null;
            return new FolderContentsRequestVM
            {
                Folder = hasFilter ? rootPath : PathHelper.Combine(rootPath, request.Folder),
                Tags = link.Tags,
                From = link.DateFrom,
                To = link.DateTo
            };
        }

        /// <summary>
        /// Returns basic contents of a folder: directly nested media, tags, subfolders.
        /// </summary>
        private async Task<FolderContentsVM> GetFolderSimpleContentsAsync(FolderContentsRequestVM request, UserContext ctx)
        {
            var folder = await _db.Folders
                                  .AsNoTracking()
                                  .Include(x => x.Tags)
                                  .ThenInclude(x => x.Tag)
                                  .GetAsync(x => x.Path == request.Folder, $"Folder ({request.Folder})");

            var canShowSubfolders = ctx.Link == null || ctx.Link.Scope == SearchScope.CurrentFolderAndSubfolders;
            var subfolders = canShowSubfolders
                ? await _db.Folders
                           .AsNoTracking()
                           .Include(x => x.Thumbnail)
                           .Where(x => x.Path.StartsWith(folder.Path) && x.Depth == folder.Depth + 1)
                           .OrderBy(x => x.Caption)
                           .ToArrayAsync()
                : new Folder[0];

            if (ctx.Link?.Folder is Folder root)
                foreach (var sf in subfolders)
                    sf.Path = sf.Path.Replace(root.Path, "");

            var media = await _db.Media
                                 .AsNoTracking()
                                 .Where(x => x.FolderKey == folder.Key)
                                 .OrderBy(x => x.Order)
                                 .ToListAsync();

            return new FolderContentsVM
            {
                Tags = ctx.Link == null ? _mapper.Map<TagBindingVM[]>(folder.Tags) : null,
                Subfolders = _mapper.Map<FolderVM[]>(subfolders),
                Media = _mapper.Map<MediaThumbnailVM[]>(media)
            };
        }

        /// <summary>
        /// Returns media by complete search.
        /// </summary>
        private async Task<FolderContentsVM> GetFolderFilteredContentsAsync(FolderContentsRequestVM request)
        {
            var folder = await _db.Folders
                                  .AsNoTracking()
                                  .Include(x => x.Tags)
                                  .GetAsync(x => x.Path == request.Folder, $"Folder ({request.Folder})");
            
            var query = _db.Media.AsNoTracking();

            foreach (var tagId in request.Tags.TryParseList<int>(","))
                query = query.Where(x => x.Tags.Any(y => y.Tag.Id == tagId));
            
            if (request.Scope == SearchScope.CurrentFolder)
                query = query.Where(x => x.FolderKey == folder.Key);
            else if (request.Scope == SearchScope.CurrentFolderAndSubfolders)
                query = query.Where(x => x.Folder.Path.StartsWith(folder.Path));

            var media = await query.OrderBy(x => x.Date)
                                   .ThenBy(x => x.Folder.Caption)
                                   .ThenBy(x => x.Order)
                                   .ToListAsync();

            var datedMedia = TryFilterMediaByDate(request, media);
            
            return new FolderContentsVM
            {
                Media = _mapper.Map<MediaThumbnailVM[]>(datedMedia)
            };
        }

        /// <summary>
        /// Applies date filtering to the list of media.
        /// </summary>
        private IReadOnlyList<Media> TryFilterMediaByDate(FolderContentsRequestVM request, IReadOnlyList<Media> source)
        {
            var dateFrom = TryParse(request.From);
            var dateTo = TryParse(request.To);

            if (dateFrom == null && dateTo == null)
                return source;

            return source.Select(x => new
                         {
                             Date = FuzzyDate.TryParse(x.Date),
                             Media = x
                         })
                         .Where(x => x.Date != null)
                         .Where(x => (dateFrom == null || x.Date >= dateFrom) && (dateTo == null || x.Date <= dateTo))
                         .Select(x => x.Media)
                         .ToList();

            FuzzyDate? TryParse(string value)
            {
                var date = value?.TryParse<DateTime?>();
                return date == null ? (FuzzyDate?) null : new FuzzyDate(date.Value);
            }
        }

        #endregion
    }
}