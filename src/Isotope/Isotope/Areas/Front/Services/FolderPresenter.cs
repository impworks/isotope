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

namespace Isotope.Areas.Front.Services;

/// <summary>
/// Service class for displaying folders and media to the user.
/// </summary>
public class FolderPresenter(AppDbContext db, IMapper mapper)
{
    #region Tree
        
    /// <summary>
    /// Returns the tree of folders available for the current user.
    /// </summary>
    public async Task<FolderVM[]> GetFolderTreeAsync(UserContext ctx)
    {
        if (ctx.Link != null)
            await db.SharedLinks
                    .Where(x => x.Key == ctx.LinkId)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.VisitCount, x => x.VisitCount + 1));

        var query = db.Folders
                       .Include(x => x.Thumbnail)
                       .AsNoTracking()
                       .Where(x => x.Depth > 0)
                       .OrderBy(x => x.Caption)
                       .AsQueryable();

        Folder baseFolder = null;
            
        if (ctx.Link is { } link)
        {
            if(link.Scope == SearchScope.CurrentFolder)
                return Array.Empty<FolderVM>();

            baseFolder = link.Folder;
            query = query.Where(x => x.Path.StartsWith(baseFolder.Path) && x.Depth > baseFolder.Depth); // current folder is root, only include nested ones
        }

        var folders = await query.ToListAsync();
        return BuildFolderTree(folders, baseFolder);
    }
        
    /// <summary>
    /// Combines the folders into a tree.
    /// </summary>
    private FolderVM[] BuildFolderTree(IList<Folder> source, Folder baseFolder)
    {
        if(source.Count == 0)
            return Array.Empty<FolderVM>();
            
        var rootDepth = source.Min(x => x.Depth);
        return source.Where(x => x.Depth == rootDepth).ToList().Select(ProcessFolder).ToArray();

        FolderVM ProcessFolder(Folder folder)
        {
            var vm = mapper.Map<FolderVM>(folder);
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
        var folder = await db.Folders
                             .AsNoTracking()
                             .Include(x => x.Thumbnail)
                             .Include(x => x.Tags)
                             .ThenInclude(x => x.Tag)
                             .GetAsync(x => x.Path == request.Folder, $"Folder ({request.Folder})");

        var canShowSubfolders = ctx.Link == null || ctx.Link.Scope == SearchScope.CurrentFolderAndSubfolders;
        var subfolders = canShowSubfolders
            ? await db.Folders
                      .AsNoTracking()
                      .Include(x => x.Thumbnail)
                      .Where(x => x.Path.StartsWith(folder.Path) && x.Depth == folder.Depth + 1)
                      .OrderBy(x => x.Caption)
                      .ToArrayAsync()
            : [];

        if (ctx.Link?.Folder is { } root)
            foreach (var sf in subfolders)
                sf.Path = sf.Path.Replace(root.Path, "");

        var media = await db.Media
                            .AsNoTracking()
                            .Where(x => x.FolderKey == folder.Key)
                            .OrderBy(x => x.Order)
                            .ToListAsync();

        return new FolderContentsVM
        {
            Caption = folder.Caption,
            ThumbnailUrl = MediaHelper.GetThumbnailUrl(folder.Thumbnail ?? media.FirstOrDefault()),
            Tags = ctx.Link == null ? mapper.Map<TagBindingVM[]>(folder.Tags) : null,
            Subfolders = mapper.Map<FolderVM[]>(subfolders),
            Media = mapper.Map<MediaThumbnailVM[]>(media)
        };
    }

    /// <summary>
    /// Returns media by complete search.
    /// </summary>
    private async Task<FolderContentsVM> GetFolderFilteredContentsAsync(FolderContentsRequestVM request)
    {
        var folder = await db.Folders
                             .AsNoTracking()
                             .Include(x => x.Tags)
                             .Include(x => x.Thumbnail)
                             .GetAsync(x => x.Path == request.Folder, $"Folder ({request.Folder})");
            
        var query = db.Media.AsNoTracking();

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
        var thumbMedia = (request.Scope is SearchScope.Everywhere ? null : folder.Thumbnail) ?? datedMedia.FirstOrDefault();
        var thumbUrl = MediaHelper.GetThumbnailUrl(thumbMedia);
            
        return new FolderContentsVM
        {
            Caption = folder.Caption,
            ThumbnailUrl = thumbUrl,
            Media = mapper.Map<MediaThumbnailVM[]>(datedMedia)
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
            return date == null ? null : new FuzzyDate(date.Value);
        }
    }

    #endregion
}