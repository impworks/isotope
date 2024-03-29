using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Impworks.Utils.Strings;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Utils;
using Isotope.Areas.Front.Dto;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Admin.Services;

/// <summary>
/// Service for managing the list of shared links.
/// </summary>
public class SharedLinkManager(AppDbContext db, IMapper mapper)
{
    /// <summary>
    /// Returns the list of all shared links known in the system.
    /// </summary>
    public async Task<IReadOnlyList<SharedLinkDetailsVM>> GetListAsync()
    {
        var links = await db.SharedLinks
                             .AsNoTracking()
                             .Include(x => x.Folder)
                             .OrderByDescending(x => x.CreationDate)
                             .ToListAsync();

        return mapper.Map<SharedLinkDetailsVM[]>(links);
    }

    /// <summary>
    /// Creates a new shared link.
    /// </summary>
    public async Task<KeyResultVM> CreateAsync(SharedLinkVM vm)
    {
        await ValidateAsync(vm);
            
        var folderPath = PathHelper.Normalize(vm.Folder);
        var folder = await db.Folders.GetAsync(x => x.Path == folderPath, $"Folder '{vm.Folder}' does not exist.");
            
        var link = new SharedLink
        {
            Key = UniqueKey.Get(),
            CreationDate = DateTime.Now,
            Folder = folder,
        };
        mapper.Map(vm, link);

        db.SharedLinks.Add(link);
        await db.SaveChangesAsync();

        return new KeyResultVM {Key = link.Key};
    }

    /// <summary>
    /// Removes the shared link.
    /// </summary>
    public async Task RemoveAsync(string key)
    {
        var link = await db.SharedLinks.GetAsync(x => x.Key == key, $"Shared link '{key}' does not exist.");
        db.SharedLinks.Remove(link);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Changes the properties of a shared link.
    /// </summary>
    public async Task UpdateAsync(string key, SharedLinkVM vm)
    {
        var link = await db.SharedLinks.GetAsync(x => x.Key == key, $"Shared link '{key}' does not exist.");
        link.Caption = vm.Caption;
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Ensures that the request is valid.
    /// </summary>
    private async Task ValidateAsync(SharedLinkVM vm)
    {
        if(!string.IsNullOrEmpty(vm.From) && vm.From.TryParse<DateTime?>() == null)
            throw new OperationException($"Date '{vm.From}' is not valid.");
            
        if(!string.IsNullOrEmpty(vm.To) && vm.To.TryParse<DateTime?>() == null)
            throw new OperationException($"Date '{vm.To}' is not valid.");
            
        if(!Enum.IsDefined(typeof(SearchScope), vm.Scope))
            throw new OperationException($"Search mode '{vm.Scope}' is not supported.");

        if (vm.Tags?.Length > 0)
        {
            var existingCount = await db.Tags.CountAsync(x => vm.Tags.Contains(x.Id));
            if(existingCount != vm.Tags.Length)
                throw new OperationException($"Some tags of the specified list '{vm.Tags}' do not exist!");
        }
    }
}