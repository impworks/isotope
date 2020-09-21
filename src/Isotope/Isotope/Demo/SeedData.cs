using System.IO;
using System.Threading.Tasks;
using Isotope.Data;
using Isotope.Data.Models;

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

            SeedCatsFolders(ctx);
            SeedTravelFolders(ctx);
            SeedEdgeCaseFolders(ctx);

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Creates folders with cute fluffy kitties :3
        /// </summary>
        private static void SeedCatsFolders(SeedContext ctx)
        {
            var catsFolder = ctx.AddFolder("Cats", "cats");

            var britsFolder = ctx.AddFolder("British", "british", catsFolder);
            for (var i = 1; i <= 4; i++)
                ctx.AddPhoto($"brit-{i}.jpg", britsFolder);

            var cat = ctx.AddPhoto("brit-5.jpg", britsFolder);

            var abyFolder = ctx.AddFolder("Abyssinian", "aby", catsFolder);
            for (var i = 1; i <= 4; i++)
                ctx.AddPhoto($"aby-{i}.jpg", abyFolder);

            var catsTag = ctx.AddTag("Felines");
            var kittyTag = ctx.AddTag("Kitty", TagType.Person);

            ctx.TagFolder(catsFolder, catsTag);

            ctx.TagPhoto(cat, kittyTag, TagBindingType.Depicted, "0.2,0.2,0.6,0.7");
        }

        /// <summary>
        /// Creates folders with travel-related pictures.
        /// </summary>
        private static void SeedTravelFolders(SeedContext ctx)
        {
            var travelFolder = ctx.AddFolder("Travel", "travel");

            var tp1 = ctx.AddPhoto("travel-1.jpg", travelFolder, "Rodos Island", "2018.09.01");
            var tp2 = ctx.AddPhoto("travel-2.jpg", travelFolder, "Simi Island", "2018.09.04");
            var tp3 = ctx.AddPhoto("travel-3.jpg", travelFolder, "Colosseum", "2018.04.22");
            var tp4 = ctx.AddPhoto("travel-4.jpg", travelFolder, "Trevi Fountain", "2018.04.23");
            var tp5 = ctx.AddPhoto("travel-5.jpg", travelFolder, "Venice", "2018.04.??");

            var greeceTag = ctx.AddTag("Greece");
            var italyTag = ctx.AddTag("Italy");
            var sightTag = ctx.AddTag("Sight-seeing");

            ctx.TagPhoto(tp1, greeceTag);
            ctx.TagPhoto(tp1, sightTag);
            ctx.TagPhoto(tp2, greeceTag);
            ctx.TagPhoto(tp3, italyTag);
            ctx.TagPhoto(tp3, sightTag);
            ctx.TagPhoto(tp4, italyTag);
            ctx.TagPhoto(tp5, italyTag);
        }

        /// <summary>
        /// Creates uncommon folders to test various edge cases.
        /// </summary>
        private static void SeedEdgeCaseFolders(SeedContext ctx)
        {
            var longFolder = ctx.AddFolder("Z Long folder", "long");
            for (var i = 1; i <= 50; i++)
                ctx.AddFolder("Subfolder " + i, "sub" + i, longFolder);

            var deepFolder = ctx.AddFolder("Z Deep folder", "deep");
            var curr = deepFolder;
            for (var i = 1; i <= 10; i++)
                curr = ctx.AddFolder("Subfolder " + i, "sub" + i, curr);
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