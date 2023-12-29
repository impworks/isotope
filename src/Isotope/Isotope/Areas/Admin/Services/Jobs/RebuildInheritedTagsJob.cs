using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Impworks.Utils.Dictionary;
using Isotope.Code.Services.Jobs;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Admin.Services.Jobs
{
    /// <summary>
    /// Background job for rebuilding tag bindings inherited by media from folders.
    /// </summary>
    public class RebuildInheritedTagsJob: JobBase
    {
        public RebuildInheritedTagsJob(AppDbContext db)
        {
            _db = db;
        }
        
        private readonly AppDbContext _db;

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            var folders = await GetFolderLookupAsync(token);
            var media = await GetMediaLookupAsync(token);
            
            var tags = new List<MediaTagBinding>();
            ProcessFolder(folders[0].Single(), ImmutableHashSet.Create<int>());

            await _db.MediaTags.RemoveWhereAsync(x => x.Type == TagBindingType.Inherited);
            _db.MediaTags.AddRange(tags);
            await _db.SaveChangesAsync(CancellationToken.None);

            void ProcessFolder(Folder folder, ImmutableHashSet<int> tagIds)
            {
                var currTagIds = folder.Tags.Select(x => x.TagId).Aggregate(tagIds, (hash, x) => hash.Add(x));
                
                var folderMedia = media.TryGetValue(folder.Key);
                if (folderMedia != null)
                {
                    tags.AddRange(
                        from mediaKey in folderMedia
                        from tagId in currTagIds
                        select new MediaTagBinding
                        {
                            TagId = tagId,
                            MediaKey = mediaKey,
                            Type = TagBindingType.Inherited
                        }
                    );
                }

                if (folders.TryGetValue(folder.Depth + 1, out var nextLevel))
                    foreach(var subfolder in nextLevel.Where(x => x.Path.StartsWith(folder.Path)))
                        ProcessFolder(subfolder, currTagIds);
            }
        }

        private async Task<FrozenDictionary<int, List<Folder>>> GetFolderLookupAsync(CancellationToken token)
        {
            var folders = await _db.Folders
                                   .AsNoTracking()
                                   .Include(x => x.Tags)
                                   .ToListAsync(token);

            return folders.GroupBy(x => x.Depth)
                          .ToFrozenDictionary(x => x.Key, x => x.ToList());
        }
        

        /// <summary>
        /// Returns the lookup for finding media files in a particular folder.
        /// </summary>
        private async Task<FrozenDictionary<string, List<string>>> GetMediaLookupAsync(CancellationToken token)
        {
            var media = await _db.Media
                                 .Select(x => new { x.Key, x.FolderKey })
                                 .ToListAsync(token);

            return media.GroupBy(x => x.FolderKey, x => x.Key)
                        .ToFrozenDictionary(x => x.Key, x => x.ToList());
        }
    }
}