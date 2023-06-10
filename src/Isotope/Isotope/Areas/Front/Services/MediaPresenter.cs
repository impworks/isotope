using System.Linq;
using System.Threading.Tasks;
using Impworks.Utils.Strings;
using Isotope.Areas.Front.Dto;
using Isotope.Code.Utils.Date;
using Isotope.Code.Utils.Exceptions;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Front.Services
{
    /// <summary>
    /// Helper class for retrieving the media details.
    /// </summary>
    public class MediaPresenter
    {
        public MediaPresenter(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        /// <summary>
        /// Returns the media details.
        /// </summary>
        public async Task<MediaVM> GetMediaAsync(string key, UserContext ctx)
        {
            var errorKey = $"Media ({key})";
            
            var media = await _db.Media
                                 .AsNoTracking()
                                 .Include(x => x.Tags)
                                 .ThenInclude(x => x.Tag)
                                 .Include(x => x.Folder)
                                 .GetAsync(x => x.Key == key, errorKey);

            if (ctx.Link != null)
            {
                // subfolders disallowed: only media immediately inside current folder are available
                if(ctx.Link.Scope == SearchScope.CurrentFolder && media.FolderKey != ctx.Link.Folder.Key)
                    throw new NotFoundException(errorKey);
                
                // inside subfolder
                if(!media.Folder.Path.StartsWith(ctx.Link.Folder.Path))
                    throw new NotFoundException(errorKey);

                // must have tags
                if (!string.IsNullOrEmpty(ctx.Link.Tags))
                {
                    var tagIds = ctx.Link.Tags.TryParseList<int>(",");
                    if(!media.Tags.Any(x => tagIds.Contains(x.TagId)))
                        throw new NotFoundException(errorKey);
                }

                var mediaDate = FuzzyDate.TryParse(media.Date);
                var dateFrom = FuzzyDate.TryParse(ctx.Link.DateFrom);
                var dateTo = FuzzyDate.TryParse(ctx.Link.DateTo);
                
                if((dateFrom != null && !(mediaDate >= dateFrom)) || (dateTo != null && !(mediaDate <= dateTo)))
                    throw new NotFoundException(errorKey);
            }

            return _mapper.Map<MediaVM>(media);
        }
    }
}