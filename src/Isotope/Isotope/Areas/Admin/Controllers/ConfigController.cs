using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for working with shared links.
    /// </summary>
    [Route("~/@api/admin/config")]
    public class ConfigController: AdminControllerBase
    {
        public ConfigController(ConfigManager cfgMgr)
        {
            _cfgMgr = cfgMgr;
        }
        
        private readonly ConfigManager _cfgMgr;

        /// <summary>
        /// Returns the current configuration state.
        /// </summary>
        [HttpGet, Route("")]
        public Task<ConfigVM> Get()
        {
            return _cfgMgr.GetAsync();
        }

        /// <summary>
        /// Updates the configuration state.
        /// </summary>
        [HttpPut, Route("")]
        public Task Set([FromBody] ConfigVM vm)
        {
            return _cfgMgr.SetAsync(vm);
        }
    }
}