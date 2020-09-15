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

        public DbSet<DynamicConfigWrapper> DynamicConfig => Set<DynamicConfigWrapper>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Media>().HasIndex(x => x.Id);
            base.OnModelCreating(builder);
        }
    }
}
