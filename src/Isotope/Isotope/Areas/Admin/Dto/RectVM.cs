using Isotope.Code.Utils;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto
{
    public class RectVM: IMapped
    {
        /// <summary>
        /// X coordinate of the left top corner (in 0..1 coordinate space).
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y coordinate of the left top corner (in 0..1 coordinate space).
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Width of the bounding box (in 0..1 coordinate space).
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Height of the bounding box (in 0..1 coordinate space).
        /// </summary>
        public double Height { get; set; }
        
        public void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<Rect, RectVM>()
                  .Map(x => x.X, x => x.X)
                  .Map(x => x.Y, x => x.Y)
                  .Map(x => x.Width, x => x.Width)
                  .Map(x => x.Height, x => x.Height);
            
            config.NewConfig<RectVM, Rect>()
                  .Map(x => x.X, x => x.X)
                  .Map(x => x.Y, x => x.Y)
                  .Map(x => x.Width, x => x.Width)
                  .Map(x => x.Height, x => x.Height);
        }
    }
}