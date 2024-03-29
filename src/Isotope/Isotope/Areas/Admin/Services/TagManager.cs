using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using TagVM = Isotope.Areas.Admin.Dto.TagVM;

namespace Isotope.Areas.Admin.Services;

/// <summary>
/// Service for managing the global list of tags.
/// </summary>
public class TagManager(AppDbContext db, IMapper mapper)
{
    /// <summary>
    /// Returns the list of all tags.
    /// </summary>
    public async Task<IReadOnlyList<TagVM>> GetListAsync()
    {
        return await db.Tags
                        .OrderBy(x => x.Caption)
                        .ProjectToType<TagVM>(mapper.Config)
                        .ToListAsync();
    }

    /// <summary>
    /// Creates a new tag.
    /// </summary>
    public async Task<TagVM> CreateAsync(TagVM vm)
    {
        await ValidateAsync(vm, null);

        var tag = mapper.Map<Tag>(vm);
        db.Tags.Add(tag);
        await db.SaveChangesAsync();

        return mapper.Map<TagVM>(tag);
    }

    /// <summary>
    /// Updates the existing tag.
    /// </summary>
    public async Task UpdateAsync(int id, TagVM vm)
    {
        await ValidateAsync(vm, id);
            
        var tag = await db.Tags.GetAsync(x => x.Id == id, $"Tag #{id} does not exist.");
        mapper.Map(vm, tag);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Removes the existing tag, clears all bindings.
    /// </summary>
    public async Task RemoveAsync(int id)
    {
        // todo: configure cascade delete
        var tag = await db.Tags.GetAsync(x => x.Id == id, $"Tag #{id} does not exist.");
        db.Tags.Remove(tag);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Ensures that the request is correct.
    /// </summary>
    private async Task ValidateAsync(TagVM vm, int? id)
    {
        if(string.IsNullOrEmpty(vm.Caption))
            throw new OperationException($"Required field {nameof(vm.Caption)} is not set.");
            
        if(!Enum.IsDefined(typeof(TagType), vm.Type))
            throw new OperationException($"Tag type '{vm.Type}' is unknown.");
            
        if(await db.Tags.AnyAsync(x => x.Caption.ToLower() == vm.Caption.ToLower() && x.Id != id))
            throw new OperationException($"Tag '{vm.Caption}' already exists.");
    }
}