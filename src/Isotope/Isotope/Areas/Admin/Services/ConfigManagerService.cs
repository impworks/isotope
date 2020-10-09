using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Code.Services.Config;
using Isotope.Data;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Isotope.Areas.Admin.Services
{
    /// <summary>
    /// Service for managing configuration properties.
    /// </summary>
    public class ConfigManagerService
    {
        public ConfigManagerService(AppDbContext db, ConfigService cfg, IMapper mapper)
        {
            _db = db;
            _cfg = cfg;
            _mapper = mapper;
        }
        
        private readonly AppDbContext _db;
        private readonly ConfigService _cfg;
        private readonly IMapper _mapper;

        /// <summary>
        /// Returns the editable config properties.
        /// </summary>
        public async Task<ConfigVM> GetAsync()
        {
            var wrapper = await _db.DynamicConfig.FirstOrDefaultAsync();
            var config = JsonConvert.DeserializeObject(wrapper.Value);
            return _mapper.Map<ConfigVM>(config);
        }

        /// <summary>
        /// Updates the current config.
        /// </summary>
        public async Task SetAsync(ConfigVM config)
        {
            var wrapper = await _db.DynamicConfig.FirstOrDefaultAsync();
            wrapper.Value = JsonConvert.SerializeObject(_mapper.Map<DynamicConfig>(config));
            await _db.SaveChangesAsync();
            
            _cfg.ResetCache();
        }
    }
}