using System;
using System.Collections.Generic;
using System.IO;
using Impworks.Utils.Linq;
using Impworks.Utils.Strings;
using Isotope.Areas.Front.Dto;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data.Models;

namespace Isotope.Data.Seed
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
            var sourceFullPath = Path.Combine(dir, "Data", "Seed", "Content", path);
            var targetFullPath = Path.Combine(dir, "wwwroot", targetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(targetFullPath));
            File.Copy(sourceFullPath, targetFullPath);
            File.Copy(sourceFullPath, MediaHelper.GetSizedMediaPath(targetFullPath, MediaSize.Small)); // no thumbnails for now
            File.Copy(sourceFullPath, MediaHelper.GetSizedMediaPath(targetFullPath, MediaSize.Large));
            
            var media = new Media
            {
                Key = key,
                Description = descr,
                Date = date,
                Folder = folder,
                FolderKey = folder.Key,
                Order = order,
                Path = "/" + targetPath,
                Type = MediaType.Photo,
                Tags = new List<MediaTagBinding>()
            };

            _db.Media.Add(media);

            return media;
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

                bind.Location = new MediaTagBindingLocation
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
        public SharedLink AddSharedLink(Folder folder = null, int[] tagIds = null, SearchMode? mode = null, string dateFrom = null, string dateTo = null)
        {
            var link = new SharedLink
            {
                Id = UniqueKey.Get(),
                Folder = folder,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Mode = mode ?? SearchMode.CurrentFolder,
                Tags = tagIds?.JoinString(",")
            };

            _db.SharedLinks.Add(link);
            
            return link;
        }
    }
}