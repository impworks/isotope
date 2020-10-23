using Isotope.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Data
{
    /// <summary>
    /// DataContext of the application.
    /// </summary>
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Folder> Folders => Set<Folder>();
        public DbSet<Media> Media => Set<Media>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<FolderTagBinding> FolderTags => Set<FolderTagBinding>();
        public DbSet<MediaTagBinding> MediaTags => Set<MediaTagBinding>();
        public DbSet<TagHash> TagHashes => Set<TagHash>();
        public DbSet<TagSuggestion> TagSuggestions => Set<TagSuggestion>();
        public DbSet<SharedLink> SharedLinks => Set<SharedLink>();

        public DbSet<DynamicConfigWrapper> DynamicConfig => Set<DynamicConfigWrapper>();
        public DbSet<JobState> JobStates => Set<JobState>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Folder>().HasOne(x => x.Thumbnail).WithMany().HasForeignKey(x => x.ThumbnailKey).IsRequired(false);
            builder.Entity<Folder>().HasIndex(x => x.Path).IsUnique(true);

            builder.Entity<Media>().HasOne(x => x.Folder).WithMany().HasForeignKey(x => x.FolderKey).IsRequired(true);
            builder.Entity<Media>().OwnsOne(x => x.ThumbnailRect);

            builder.Entity<SharedLink>().HasOne(x => x.Folder).WithMany().IsRequired(false);

            builder.Entity<FolderTagBinding>().HasOne(x => x.Folder).WithMany(x => x.Tags).IsRequired(true);
            builder.Entity<FolderTagBinding>().HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId).IsRequired(true);

            builder.Entity<MediaTagBinding>().HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId).IsRequired(true);
            builder.Entity<MediaTagBinding>().HasOne(x => x.Media).WithMany(x => x.Tags).HasForeignKey(x => x.MediaKey).IsRequired(true);
            builder.Entity<MediaTagBinding>().OwnsOne(x => x.Location);

            builder.Entity<TagSuggestion>().HasOne(x => x.Media).WithMany().IsRequired(true);
            builder.Entity<TagSuggestion>().OwnsOne(x => x.Location);
            
            base.OnModelCreating(builder);
        }
    }
}
