using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
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
            var folder = await _db.Folders.GetAsync(x => x.Path == vm.Folder, $"Folder '{vm.Folder}' does not exist.");
            
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
        public async Task DeleteAsync(string key)
        {
            var link = await _db.SharedLinks.GetAsync(x => x.Key == key, $"Shared link '{key}' does not exist.");
            _db.SharedLinks.Remove(link);
            await _db.SaveChangesAsync();
        }
    }
}