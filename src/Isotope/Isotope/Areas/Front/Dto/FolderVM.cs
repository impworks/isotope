using System.Collections.Generic;
using Isotope.Code.Utils;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Details of a folder.
    /// </summary>
    public class FolderVM: IMapped
    {
        /// <summary>
        /// Readable name of the folder.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Full path of this folder from root.
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// Full path to a thumbnail image for this folder.
        /// May be empty.
        /// </summary>
        public string ThumbnailPath { get; set; }
        
        /// <summary>
        /// Number of media files in the folder.
        /// </summary>
        public int MediaCount { get; set; }
        
        /// <summary>
        /// List of direct subfolders (when in tree).
        /// </summary>
        public IReadOnlyList<FolderVM> Subfolders { get; set; }

        public void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<Folder, FolderVM>()
                  .Map(x => x.Caption, x => x.Caption)
                  .Map(x => x.Path, x => x.Path)
                  .Map(x => x.ThumbnailPath, x => x.Thumbnail.Path)
                  .Map(x => x.MediaCount, x => x.MediaCount)
                  .Ignore(x => x.Subfolders);
        }
    }
}