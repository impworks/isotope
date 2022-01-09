using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Impworks.Utils.Linq;
using Impworks.Utils.Strings;
using Isotope.Areas.Front.Dto;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;

namespace Isotope.Demo
{
    /// <summary>
    /// Helper class for fluently seeding default data.
    /// </summary>
    public class SeedContext
    {
        public SeedContext(AppDbContext db)
        {
            _db = db;
            _mediaOrder = new Dictionary<string, int>();
        }
        
        private readonly AppDbContext _db;
        private readonly Dictionary<string, int> _mediaOrder;

        /// <summary>
        /// Adds a new folder to the context.
        /// </summary>
        public async Task<Folder> AddFolderAsync(string caption, string slug, Folder parent = null)
        {
            var folder = new Folder
            {
                Key = UniqueKey.Get(),
                Caption = caption,
                Slug = slug,
                Tags = new List<FolderTagBinding>(),
                Thumbnail = null
            };

            if (parent == null)
            {
                folder.Depth = 1;
                folder.Path = PathHelper.Normalize(slug);
            }
            else
            {
                folder.Depth = parent.Depth + 1;
                folder.Path = PathHelper.Combine(parent.Path, slug);
            }

            _db.Folders.Add(folder);
            await _db.SaveChangesAsync();

            return folder;
        }

        /// <summary>
        /// Adds a new photo to the context.
        /// </summary>
        public async Task<Media> AddPhotoAsync(string path, Folder folder, string descr = null, string date = null)
        {
            var key = UniqueKey.Get();
            var order = _mediaOrder.ContainsKey(folder.Key)
                ? _mediaOrder[folder.Key]++
                : (_mediaOrder[folder.Key] = 1);
            
            var targetPath = Path.Combine("@media", folder.Key, key + ".jpg");

            var dir = Directory.GetCurrentDirectory();
            var sourceFullPath = Path.Combine(dir, "Demo", "Media", path);
            var targetFullPath = Path.Combine(dir, "App_Data", targetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(targetFullPath));

            File.Copy(sourceFullPath, targetFullPath);
            using var srcImg = await Image.LoadAsync(sourceFullPath);
            await ImageHelper.CreateThumbnailsAsync(srcImg, targetFullPath);

            var media = new Media
            {
                Key = key,
                UploadDate = DateTime.Now,
                VersionDate = DateTime.Now,
                Description = descr,
                Date = date,
                Folder = folder,
                FolderKey = folder.Key,
                Order = order,
                Path = $"/@media/{folder.Key}/{key}.jpg",
                ThumbnailRect = ImageHelper.GetDefaultThumbnailRect(srcImg.Size()),
                IsReady = true,
                Width = srcImg.Width,
                Height = srcImg.Height,
                Type = MediaType.Photo,
                Tags = new List<MediaTagBinding>()
            };

            folder.MediaCount++;

            if (folder.Thumbnail == null)
                folder.Thumbnail = media;

            _db.Media.Add(media);
            await _db.SaveChangesAsync();

            return media;
        }

        /// <summary>
        /// Adds a new tag.
        /// </summary>
        public async Task<Tag> AddTagAsync(string caption, TagType type = TagType.Custom)
        {
            var tag = new Tag
            {
                Caption = caption,
                Hashes = new List<TagHash>(),
                Type = type
            };

            _db.Tags.Add(tag);
            await _db.SaveChangesAsync();

            return tag;
        }

        /// <summary>
        /// Adds a tag to the photo.
        /// </summary>
        public async Task TagPhotoAsync(Media photo, Tag tag, TagBindingType type = TagBindingType.Custom, string location = null)
        {
            var bind = new MediaTagBinding
            {
                Media = photo,
                Tag = tag,
                Type = type
            };

            if (location != null)
            {
                var parts = location.TryParseList<double>();
                if(parts.Count != 4)
                    throw new ArgumentException("Invalid location syntax: expected 4 floats separated by commas.");

                bind.Location = new Rect
                {
                    X = parts[0],
                    Y = parts[1],
                    Width = parts[2],
                    Height = parts[3]
                };
            }

            _db.MediaTags.Add(bind);
            photo.Tags.Add(bind);
            await _db.SaveChangesAsync();
        }
        
        /// <summary>
        /// Adds a tag to the folder.
        /// </summary>
        public async Task TagFolderAsync(Folder folder, Tag tag)
        {
            var bind = new FolderTagBinding
            {
                Folder = folder,
                Tag = tag
            };

            _db.FolderTags.Add(bind);
            folder.Tags.Add(bind);

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a new shared link.
        /// </summary>
        public async Task<SharedLink> AddSharedLink(Folder folder = null, int[] tagIds = null, SearchScope? mode = null, string dateFrom = null, string dateTo = null, string key = null)
        {
            var link = new SharedLink
            {
                Key = key ?? UniqueKey.Get(),
                CreationDate = DateTime.Now,
                Folder = folder,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Scope = mode ?? SearchScope.CurrentFolder,
                Tags = tagIds?.JoinString(",")
            };

            _db.SharedLinks.Add(link);
            await _db.SaveChangesAsync();
            
            return link;
        }

        /// <summary>
        /// Creates all media tags inherited from folders.
        /// </summary>
        public async Task ApplyInheritedTagsAsync()
        {
            var foldersList = await _db.Folders
                                       .AsNoTracking()
                                       .Include(x => x.Tags)
                                       .Where(x => x.Depth != 0)
                                       .ToListAsync();
            var folders = foldersList.ToLookup(x => x.Depth, x => x);

            var media = await _db.Media
                                 .Select(x => new {x.Folder, x.Key})
                                 .ToListAsync();

            foreach (var m in media)
            {
                var tagIds = new HashSet<int>();
                var f = m.Folder;
                while (f != null)
                {
                    tagIds.AddRange(f.Tags.Select(x => x.Id));
                    f = f.Depth > 1
                        ? folders[f.Depth - 1].FirstOrDefault(x => f.Path.StartsWith(x.Path))
                        : null;
                }

                _db.MediaTags.AddRange(tagIds.Select(x => new MediaTagBinding
                {
                    TagId = x,
                    MediaKey = m.Key,
                    Type = TagBindingType.Inherited
                }));
            }
            
            await _db.SaveChangesAsync();
        }
    }
}