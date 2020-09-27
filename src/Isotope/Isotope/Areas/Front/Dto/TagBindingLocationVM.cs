using Isotope.Code.Utils;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Location of a marked entity on a photo.
    /// </summary>
    public class TagBindingLocationVM: IMapped
    {
        /// <summary>
        /// X coordinate of the top left box edge (in 0..1 coordinates).
        /// </summary>
        public double X { get; set; }
        
        /// <summary>
        /// Y coordinate of the top left box edge (in 0..1 coordinates).
        /// </summary>
        public double Y { get; set; }
        
        /// <summary>
        /// Width of the box (in 0..1 coordinates).
        /// </summary>
        public double Width { get; set; }
        
        /// <summary>
        /// Height of the box (in 0..1 coordinates).
        /// </summary>
        public double Height { get; set; }

        public void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<Rect, TagBindingLocationVM>()
                  .Map(x => x.X, x => x.X)
                  .Map(x => x.Y, x => x.Y)
                  .Map(x => x.Width, x => x.Width)
                  .Map(x => x.Height, x => x.Height);
        }
    }
}