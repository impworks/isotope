using System;
using System.Collections.Generic;
using System.Drawing;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        public Folder AddFolder(string caption, string slug, Folder parent = null)
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

            return folder;
        }

        /// <summary>
        /// Adds a new photo to the context.
        /// </summary>
        public Media AddPhoto(string path, Folder folder, string descr = null, string date = null)
        {
            var key = UniqueKey.Get();
            var order = _mediaOrder.ContainsKey(folder.Key)
                ? _mediaOrder[folder.Key]++
                : (_mediaOrder[folder.Key] = 1);
            
            var targetPath = Path.Combine("@media", folder.Key, key + ".jpg");

            var dir = Directory.GetCurrentDirectory();
            var sourceFullPath = Path.Combine(dir, "Demo", "Media", path);
            var targetFullPath = Path.Combine(dir, "wwwroot", targetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(targetFullPath));

            File.Copy(sourceFullPath, targetFullPath);
            using var srcImg = Image.FromFile(sourceFullPath);
            SaveThumbnail(srcImg, targetFullPath, MediaSize.Small);
            SaveThumbnail(srcImg, targetFullPath, MediaSize.Large);

            var media = new Media
            {
                Key = key,
                Description = descr,
                Date = date,
                Folder = folder,
                FolderKey = folder.Key,
                Order = order,
                Path = $"/@media/{folder.Key}/{key}.jpg",
                ThumbnailRect = GetThumbnailRect(srcImg),
                Width = srcImg.Width,
                Height = srcImg.Height,
                Type = MediaType.Photo,
                Tags = new List<MediaTagBinding>()
            };

            folder.MediaCount++;
            _db.Media.Add(media);

            return media;

            static void SaveThumbnail(Image img, string target, MediaSize size)
            {
                using var destImg = size == MediaSize.Small
                    ? ImageHelper.ResizeToFill(img, ImageHelper.Sizes[size])
                    : ImageHelper.ResizeToFit(img, ImageHelper.Sizes[size]);
                var targetSized = MediaHelper.GetSizedMediaPath(target, size);
                destImg.Save(targetSized);
            }

            static Rect GetThumbnailRect(Image img)
            {
                var size = ImageHelper.Sizes[MediaSize.Small];
                var rect = ImageHelper.GetFillRectangle(img.Size, size);
                var w = 1.0 / img.Width;
                var h = 1.0 / img.Height;
                return new Rect
                {
                    X = rect.X * w,
                    Y = rect.Y * h,
                    Width = rect.Width * w,
                    Height = rect.Height * h,
                };
            }
        }

        /// <summary>
        /// Adds a new tag.
        /// </summary>
        public Tag AddTag(string caption, TagType type = TagType.Custom)
        {
            var tag = new Tag
            {
                Caption = caption,
                Hashes = new List<TagHash>(),
                Type = type
            };

            _db.Tags.Add(tag);

            return tag;
        }

        /// <summary>
        /// Adds a tag to the photo.
        /// </summary>
        public void TagPhoto(Media photo, Tag tag, TagBindingType type = TagBindingType.Custom, string location = null)
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
        }
        
        /// <summary>
        /// Adds a tag to the folder.
        /// </summary>
        public void TagFolder(Folder folder, Tag tag)
        {
            var bind = new FolderTagBinding
            {
                Folder = folder,
                Tag = tag
            };

            _db.FolderTags.Add(bind);
            folder.Tags.Add(bind);
        }

        /// <summary>
        /// Adds a new shared link.
        /// </summary>
        public SharedLink AddSharedLink(Folder folder = null, int[] tagIds = null, SearchMode? mode = null, string dateFrom = null, string dateTo = null, string key = null)
        {
            var link = new SharedLink
            {
                Key = key ?? UniqueKey.Get(),
                Folder = folder,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Mode = mode ?? SearchMode.CurrentFolder,
                Tags = tagIds?.JoinString(",")
            };

            _db.SharedLinks.Add(link);
            
            return link;
        }

        /// <summary>
        /// Commits current changes.
        /// </summary>
        public void SaveChanges()
        {
            _db.SaveChanges();
        }

        /// <summary>
        /// Creates all media tags inherited from folders.
        /// </summary>
        public void ApplyInheritedTags()
        {
            _db.SaveChanges();
            
            var folders = _db.Folders
                             .AsNoTracking()
                             .Include(x => x.Tags)
                             .Where(x => x.Depth != 0)
                             .ToLookup(x => x.Depth, x => x);

            var media = _db.Media
                           .Select(x => new {x.Folder, x.Key})
                           .ToList();

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
            
            _db.SaveChanges();
        }
    }
}