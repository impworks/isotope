namespace Isotope.Data.Models
{
    /// <summary>
    /// Rectangle in relative coordinates.
    /// </summary>
    public class Rect
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
    }
}