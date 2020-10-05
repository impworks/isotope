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

namespace Isotope.Areas.Admin.Services
{
    /// <summary>
    /// Service for managing the list of shared links.
    /// </summary>
    public class SharedLinkManagerService
    {
        public SharedLinkManagerService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        
        /// <summary>
        /// Returns the list of all shared links known in the system.
        /// </summary>
        public async Task<IReadOnlyList<SharedLinkDetailsVM>> GetListAsync()
        {
            var links = await _db.SharedLinks
                                 .AsNoTracking()
                                 .Include(x => x.Folder)
                                 .OrderByDescending(x => x.CreationDate)
                                 .ToListAsync();

            return _mapper.Map<SharedLinkDetailsVM[]>(links);
        }

        /// <summary>
        /// Creates a new shared link.
        /// </summary>
        public async Task<KeyResultVM> CreateAsync(SharedLinkVM vm)
        {
            await ValidateAsync(vm);
            
            var folderPath = PathHelper.Normalize(vm.Folder);
            var folder = await _db.Folders.GetAsync(x => x.Path == folderPath, $"Folder '{vm.Folder}' does not exist.");
            
            var link = new SharedLink
            {
                Key = UniqueKey.Get(),
                Folder = folder,
            };
            _mapper.Map(vm, link);
            
            _db.SharedLinks.Add(link);
            await _db.SaveChangesAsync();

            return new KeyResultVM {Key = link.Key};
        }

        /// <summary>
        /// Removes the shared link.
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            var link = await _db.SharedLinks.GetAsync(x => x.Key == key, $"Shared link '{key}' does not exist.");
            _db.SharedLinks.Remove(link);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Ensures that the request is valid.
        /// </summary>
        private async Task ValidateAsync(SharedLinkVM vm)
        {
            if(!string.IsNullOrEmpty(vm.DateFrom) && vm.DateFrom.TryParse<DateTime>() == null)
                throw new OperationException($"Date '{vm.DateFrom}' is not valid.");
            
            if(!string.IsNullOrEmpty(vm.DateTo) && vm.DateTo.TryParse<DateTime>() == null)
                throw new OperationException($"Date '{vm.DateTo}' is not valid.");
            
            if(!Enum.IsDefined(typeof(SearchMode), vm.Mode))
                throw new OperationException($"Search mode '{vm.Mode}' is not supported.");

            if (!string.IsNullOrEmpty(vm.Tags))
            {
                var tagIdsRaw = vm.Tags.Split(",");
                var tagIds = vm.Tags.TryParseList<int>(",");
                if(tagIdsRaw.Length != tagIds.Count)
                    throw new OperationException($"Tags format is invalid: '{vm.Tags}' is not a comma-separated list of integers.");

                var existingCount = await _db.Tags.CountAsync(x => tagIds.Contains(x.Id));
                if(existingCount != tagIds.Count)
                    throw new OperationException($"Some tags of the specified list '{vm.Tags}' do not exist!");
            }
        }
    }
}