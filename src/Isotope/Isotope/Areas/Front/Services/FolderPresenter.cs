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
                           .AsNoTracking()
                           .Where(x => x.Depth > 0)
                           .OrderBy(x => x.Caption)
                           .AsQueryable();

            Folder baseFolder = null;
            
            if (ctx.Link is SharedLink link)
            {
                if(link.Mode == SearchMode.CurrentFolder)
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
            var hasFilter = req.Tags != null || req.DateFrom != null || req.DateTo != null;
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
                return request;

            var link = ctx.Link;
            var rootPath = link.Folder.Path;
            var hasFilter = link.Tags != null || link.DateFrom != null || link.DateTo != null;
            return new FolderContentsRequestVM
            {
                Folder = hasFilter ? rootPath : PathHelper.Combine(rootPath, request.Folder),
                Tags = link.Tags,
                DateFrom = link.DateFrom,
                DateTo = link.DateTo
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

            var canShowSubfolders = ctx.Link == null || ctx.Link.Mode == SearchMode.CurrentFolderAndSubfolders;
            var subfolders = canShowSubfolders
                ? await _db.Folders
                           .AsNoTracking()
                           .Where(x => x.Path.StartsWith(folder.Path) && x.Depth == folder.Depth + 1)
                           .OrderBy(x => x.Caption)
                           .ToArrayAsync()
                : new Folder[0];

            var media = await _db.Media
                                 .AsNoTracking()
                                 .Where(x => x.FolderKey == folder.Key)
                                 .OrderBy(x => x.Order)
                                 .ToListAsync();

            return new FolderContentsVM
            {
                Tags = _mapper.Map<TagBindingVM[]>(folder.Tags),
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
            
            var mediaQuery = _db.Media.AsNoTracking();
            
            var tagIds = request.Tags.TryParseList<int>(",");
            if (tagIds.Any())
            {
                var mediaCondition = GetMatchingMediaCondition(request, folder, tagIds);
                var folderCondition = await GetMatchingFoldersConditionAsync(request, tagIds);

                var condition = folderCondition == null
                    ? mediaCondition
                    : ExprHelper.Or(folderCondition, mediaCondition);

                mediaQuery = mediaQuery.Where(condition);
            }

            var media = await mediaQuery.OrderBy(x => x.Order)
                                        .ToListAsync();

            var datedMedia = TryFilterMediaByDate(request, media);
            
            return new FolderContentsVM
            {
                Media = _mapper.Map<MediaThumbnailVM[]>(datedMedia)
            };
        }

        /// <summary>
        /// Returns a condition that matches media by tags on containing folders.
        /// </summary>
        private async Task<Expression<Func<Media, bool>>> GetMatchingFoldersConditionAsync(FolderContentsRequestVM request, IReadOnlyList<int> tagIds)
        {
            if (request.SearchMode == SearchMode.CurrentFolder)
                return null;

            var query = _db.Folders
                           .AsNoTracking()
                           .Where(x => x.Tags.Any(y => tagIds.Contains(y.Tag.Id)));  

            if (request.SearchMode == SearchMode.CurrentFolderAndSubfolders)
                query = query.Where(x => x.Path.StartsWith(request.Folder));

            var matchedFolderPaths = await query.Select(x => x.Path).ToListAsync();
            if (!matchedFolderPaths.Any())
                return null;

            var allSubfolders = await _db.Folders.Select(x => new {x.Key, x.Path}).ToListAsync(); // sic! retrieving all folders and checking on client side
            var folderKeys = allSubfolders.Where(x => matchedFolderPaths.Any(y => x.Path.StartsWith(y)))
                                          .Select(x => x.Key)
                                          .ToList();
            return x => folderKeys.Contains(x.FolderKey);
        }

        /// <summary>
        /// Return a condition that matches media by tags.
        /// </summary>
        private Expression<Func<Media, bool>> GetMatchingMediaCondition(FolderContentsRequestVM request, Folder folder, IReadOnlyList<int> tagIds)
        {
            Expression<Func<Media, bool>> condition = x => x.Tags.Any(y => tagIds.Contains(y.Tag.Id));

            if (request.SearchMode == SearchMode.CurrentFolder)
                condition = ExprHelper.And(condition, x => x.FolderKey == folder.Key);
            else if (request.SearchMode == SearchMode.CurrentFolderAndSubfolders)
                condition = ExprHelper.And(condition, x => x.Folder.Path.StartsWith(folder.Path));

            return condition;
        }

        /// <summary>
        /// Applies date filtering to the list of media.
        /// </summary>
        private IReadOnlyList<Media> TryFilterMediaByDate(FolderContentsRequestVM request, IReadOnlyList<Media> source)
        {
            var dateFrom = FuzzyDate.TryParse(request.DateFrom);
            var dateTo = FuzzyDate.TryParse(request.DateTo);

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
        }

        #endregion
    }
}