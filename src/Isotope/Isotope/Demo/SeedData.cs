using System.IO;
using System.Threading.Tasks;
using Isotope.Data;

namespace Isotope.Demo
{
    /// <summary>
    /// Collection of seed methods.
    /// </summary>
    public class SeedData
    {
        /// <summary>
        /// Creates sample data.
        /// </summary>
        public static async Task SeedSampleDataAsync(AppDbContext db)
        {
            var ctx = new SeedContext(db);
            
            var catsFolder = ctx.AddFolder("Cats", "cats");
            var c1 = ctx.AddFolder("British", "british", catsFolder);
            var c2 = ctx.AddFolder("Abyssinian", "aby", catsFolder);

            var catsTag = ctx.AddTag("Felines");

            var longFolder = ctx.AddFolder("Long folder", "long");
            for (var i = 1; i <= 50; i++)
                ctx.AddFolder("Subfolder " + i, "sub" + i, longFolder);

            var deepFolder = ctx.AddFolder("Deep folder", "deep");
            var curr = deepFolder;
            for (var i = 1; i <= 10; i++)
                curr = ctx.AddFolder("Subfolder " + i, "sub" + i, curr);
            
            ctx.TagFolder(catsFolder, catsTag);

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes existing media and database.
        /// </summary>
        public static async Task ClearPreviousData()
        {
            var dir = Directory.GetCurrentDirectory();
            var mediaPath = Path.Combine(dir, "wwwroot", "@media");
            if (Directory.Exists(mediaPath))
            {
                Directory.Delete(mediaPath, true);
                Directory.CreateDirectory(mediaPath);
            }

            var dataPath = Path.Combine(dir, "Storage", "Isotope.db");
            if(File.Exists(dataPath))
                File.Delete(dataPath);
        }
    }
}