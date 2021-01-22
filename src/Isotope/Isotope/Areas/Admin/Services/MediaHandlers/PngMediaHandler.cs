using System.Drawing;
using System.Threading.Tasks;
using Isotope.Data.Models;

namespace Isotope.Areas.Admin.Services.MediaHandlers
{
    /// <summary>
    /// Media handler for PNG images.
    /// </summary>
    public class PngMediaHandler: IMediaHandler
    {
        /// <summary>
        /// Mime type.
        /// </summary>
        public string[] SupportedMimeTypes => new[] {"image/png"};

        /// <summary>
        /// Media processing.
        /// </summary>
        public async Task<MediaInfo> ProcessAsync(string key, string path)
        {
            var image = await Task.Run(() => Image.FromFile(path));
            return new MediaInfo
            {
                FullImage = image,
                IsReady = true,
                MediaType = MediaType.Photo
            };
        }
    }
}
