using System;
using System.Linq;
using System.Threading.Tasks;
using Isotope.Areas.Front.Dto;
using Isotope.Data;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Front.Services;

/// <summary>
/// Helper class for retrieving the list of tags.
/// </summary>
public class TagsPresenter(AppDbContext db, IMapper mapper)
{
    /// <summary>
    /// Returns the list of all tags in alphabetic order.
    /// </summary>
    public async Task<TagVM[]> GetTagsAsync(UserContext ctx)
    {
        if (ctx.Link != null)
            return Array.Empty<TagVM>();

        return await db.Tags
                       .OrderBy(x => x.Caption)
                       .ProjectToType<TagVM>(mapper.Config)
                       .ToArrayAsync();
    }
}