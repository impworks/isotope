namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Location of a marked entity on a photo.
    /// </summary>
    public class TagBindingLocationVM
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
    }
}