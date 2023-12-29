using System;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Code.Services.Config;
using Isotope.Data;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Isotope.Areas.Admin.Services;

/// <summary>
/// Service for managing configuration properties.
/// </summary>
public class ConfigManager(AppDbContext db, ConfigService cfg, IMapper mapper)
{
    /// <summary>
    /// Returns the editable config properties.
    /// </summary>
    public async Task<ConfigVM> GetAsync()
    {
        var wrapper = await db.DynamicConfig.FirstOrDefaultAsync() ?? throw new Exception("DynamicConfig not found.");
        var config = JsonConvert.DeserializeObject<DynamicConfig>(wrapper.Value)!;
        return mapper.Map<ConfigVM>(config);
    }

    /// <summary>
    /// Updates the current config.
    /// </summary>
    public async Task SetAsync(ConfigVM config)
    {
        var wrapper = await db.DynamicConfig.FirstOrDefaultAsync() ?? throw new Exception("DynamicConfig not found.");
        wrapper.Value = JsonConvert.SerializeObject(mapper.Map<DynamicConfig>(config));
        await db.SaveChangesAsync();
            
        cfg.ResetCache();
    }
}