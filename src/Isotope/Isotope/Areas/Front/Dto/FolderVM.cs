using System.Collections.Generic;

namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class FolderVM
    {
        public string Caption { get; set; }
        public string Path { get; set; }
        public string ThumbnailPath { get; set; }
        public int MediaCount { get; set; }
        
        public IReadOnlyList<FolderVM> Subfolders { get; set; }
    }
}