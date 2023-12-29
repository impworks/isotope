using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Impworks.Utils.Linq;
using Impworks.Utils.Strings;
using Isotope.Areas.Front.Dto;
using Isotope.Code.Services.Config;
using Microsoft.AspNetCore.Http;

namespace Isotope.Areas.Front.Services;

public class OpenGraphPresenter(UserContextManager userMgr, FolderPresenter folderPresenter, ConfigService config)
{
    public async Task<string> GetOpenGraphDataAsync(HttpContext ctx)
    {
        var tags = new List<string>();
        
        tags.Add($"<title>{config.GetDynamicConfig().Title}</title>");
        
        try
        {
            var userContext = await userMgr.GetUserContextAsync(ctx, true);
            var req = new FolderContentsRequestVM {Folder = ctx.Request.Path.Value};
            var contents = await folderPresenter.GetFolderContentsAsync(req, userContext);

            var image =  contents.Media.FirstOrDefault()?.ThumbnailPath;
            if (!string.IsNullOrEmpty(image))
            {
                var largeImage = image.Replace(".sm.jpg", ".lg.jpg");
                tags.Add($@"<meta property=""og:image"" content=""{largeImage}"" />");
            }

            var caption = System.Web.HttpUtility.HtmlEncode(StringHelper.Coalesce(userContext.Link?.Caption, contents.Caption));
            if(!string.IsNullOrEmpty(caption))
                tags.Add($@"<meta name=""description"" content=""{caption}"" />");
        }
        catch
        {
            // do nothing
        }

        return tags.JoinString("");
    }
}