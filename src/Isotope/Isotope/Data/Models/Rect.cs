using System;

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

        /// <summary>
        /// Checks if the two rectangles are equal.
        /// </summary>
        public bool Equals(Rect rect)
        {
            return Eq(X, rect.X)
                && Eq(Y, rect.Y)
                && Eq(Width, rect.Width)
                && Eq(Height, rect.Height);
            
            bool Eq(double a, double b) => Math.Abs(a - b) < 0.00001;
        }
    }
}