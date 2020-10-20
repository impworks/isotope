using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Impworks.Utils.Linq;
using Isotope.Areas.Front.Dto;
using Isotope.Data;
using Isotope.Data.Models;
using Microsoft.AspNetCore.Identity;

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
        public static async Task SeedSampleDataAsync(AppDbContext db, UserManager<AppUser> userMgr)
        {
            var ctx = new SeedContext(db);

            SeedCatsFolders(ctx);
            SeedTravelFolders(ctx);
            SeedEdgeCaseFolders(ctx);
            
            ctx.ApplyInheritedTags();

            await SeedDefaultUserAsync(userMgr);

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

            var catsPhoto = ctx.AddPhoto("kittens-1.jpg", catsFolder, descr: "Three kittens on a sleeping mat", date: "2020.10.??");
            ctx.TagPhoto(catsPhoto, ctx.AddTag("Alpha", TagType.Person), TagBindingType.Depicted, Calc(720, 450, 58, 38, 233, 205));
            ctx.TagPhoto(catsPhoto, ctx.AddTag("Bravo", TagType.Person), TagBindingType.Depicted, Calc(720, 450, 267, 25, 435, 192));
            ctx.TagPhoto(catsPhoto, ctx.AddTag("Charlie", TagType.Person), TagBindingType.Depicted, Calc(720, 450, 385, 150, 585, 332));
            ctx.TagPhoto(catsPhoto, ctx.AddTag("Mutts"));

            ctx.AddSharedLink(catsFolder, key: "all-cats", mode: SearchMode.CurrentFolderAndSubfolders);
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
            
            ctx.SaveChanges();

            ctx.AddSharedLink(travelFolder, new[] {greeceTag.Id}, key: "travel-greece");
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
        /// Creates a sample user to test authorization.
        /// </summary>
        private static async Task SeedDefaultUserAsync(UserManager<AppUser> userMgr)
        {
            var user = new AppUser {UserName = "admin@example.com"};
            await userMgr.CreateAsync(user, "123456");
            await userMgr.AddToRoleAsync(user, nameof(UserRole.Admin));
        }

        /// <summary>
        /// Calculates the relative coordinates from absolute ones.
        /// </summary>
        private static string Calc(int width, int height, int x1, int y1, int x2, int y2)
        {
            var x = (double) x1 / width;
            var y = (double) y1 / height;
            var w = (double) (x2 - x1) / width;
            var h = (double) (y2 - y1) / height;

            return new[] {x, y, w, h}.Select(p => p.ToString("0.000", CultureInfo.InvariantCulture)).JoinString(",");
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