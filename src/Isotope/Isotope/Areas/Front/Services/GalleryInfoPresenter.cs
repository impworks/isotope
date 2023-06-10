using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Impworks.Utils.Strings;
using Isotope.Areas.Front.Dto;
using Isotope.Code.Services.Config;

namespace Isotope.Areas.Front.Services
{
    /// <summary>
    /// Presenter service for returning basic gallery info.
    /// </summary>
    public class GalleryInfoPresenter
    {
        public GalleryInfoPresenter(ConfigService cfg)
        {
            _cfg = cfg;
        }

        private readonly ConfigService _cfg;

        /// <summary>
        /// Returns the details about this gallery.
        /// </summary>
        public async Task<GalleryInfoVM> GetInfoAsync(UserContext ctx)
        {
            var cfg = _cfg.GetDynamicConfig();

            bool? linkValid = null;
            if (!string.IsNullOrEmpty(ctx.LinkId))
                linkValid = ctx.Link != null;
            
            return new GalleryInfoVM
            {
                Caption = cfg.Title,
                Subcaption = StringHelper.Coalesce(ctx.Link?.Caption, cfg.Title),
                AllowGuests = cfg.AllowGuests,
                IsAuthorized = ctx.User != null || linkValid == true,
                IsAdmin = ctx.User?.IsAdmin,
                IsLinkValid = linkValid
            };
        }
    }
}