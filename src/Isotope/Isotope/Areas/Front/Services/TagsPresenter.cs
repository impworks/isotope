using System.Linq;
using System.Threading.Tasks;
using Isotope.Areas.Front.Dto;
using Isotope.Data;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Front.Services
{
    /// <summary>
    /// Helper class for retrieving the list of tags.
    /// </summary>
    public class TagsPresenter
    {
        public TagsPresenter(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        /// <summary>
        /// Returns the list of all tags in alphabetic order.
        /// </summary>
        public async Task<TagVM[]> GetTagsAsync(UserContext ctx)
        {
            if(ctx.Link != null)
                return new TagVM[0];
            
            return await _db.Tags
                            .OrderBy(x => x.Caption)
                            .ProjectToType<TagVM>(_mapper.Config)
                            .ToArrayAsync();
        }
    }
}