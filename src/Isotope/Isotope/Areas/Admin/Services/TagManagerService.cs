using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Utils;
using Isotope.Data;
using Isotope.Data.Models;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Admin.Services
{
    /// <summary>
    /// Service for managing the global list of tags.
    /// </summary>
    public class TagManagerService
    {
        public TagManagerService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        /// <summary>
        /// Returns the list of all tags.
        /// </summary>
        public async Task<IReadOnlyList<TagVM>> GetAsync()
        {
            return await _db.Tags
                            .OrderBy(x => x.Caption)
                            .ProjectToType<TagVM>(_mapper.Config)
                            .ToListAsync();
        }

        /// <summary>
        /// Creates a new tag.
        /// </summary>
        public async Task<TagVM> CreateAsync(TagVM vm)
        {
            if(await _db.Tags.AnyAsync(x => x.Caption == vm.Caption))
                throw new OperationException($"Tag '{vm.Caption}' already exists.");

            var tag = _mapper.Map<Tag>(vm);
            _db.Tags.Add(tag);
            await _db.SaveChangesAsync();

            return _mapper.Map<TagVM>(tag);
        }

        /// <summary>
        /// Updates the existing tag.
        /// </summary>
        public async Task UpdateAsync(int id, TagVM vm)
        {
            if(await _db.Tags.AnyAsync(x => x.Caption == vm.Caption && x.Id != id))
                throw new OperationException($"Tag '{vm.Caption}' already exists.");

            var tag = await _db.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if(tag == null)
                throw new OperationException($"Tag #{id} does not exist.");

            _mapper.Map(vm, tag);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the existing tag, clears all bindings.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            // todo: configure cascade delete
            var tag = await _db.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if(tag == null)
                throw new OperationException($"Tag #{id} does not exist.");
            
            _db.Tags.Remove(tag);
            await _db.SaveChangesAsync();
        }
    }
}