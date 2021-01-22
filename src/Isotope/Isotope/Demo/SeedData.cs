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
        public static async Task SeedSampleDataAsync(AppDbContext db)
        {
            var ctx = new SeedContext(db);

            await SeedCatsFoldersAsync(ctx);
            await SeedTravelFoldersAsync(ctx);
            // SeedEdgeCaseFolders(ctx);

            await ctx.ApplyInheritedTagsAsync();
        }

        /// <summary>
        /// Creates folders with cute fluffy kitties :3
        /// </summary>
        private static async Task SeedCatsFoldersAsync(SeedContext ctx)
        {
            var catsFolder = await ctx.AddFolderAsync("Cats", "cats");

            var britsFolder = await ctx.AddFolderAsync("British", "british", catsFolder);
            for (var i = 1; i <= 4; i++)
                await ctx.AddPhotoAsync($"brit-{i}.jpg", britsFolder);

            var cat = await ctx.AddPhotoAsync("brit-5.jpg", britsFolder);

            var abyFolder = await ctx.AddFolderAsync("Abyssinian", "aby", catsFolder);
            for (var i = 1; i <= 4; i++)
                await ctx.AddPhotoAsync($"aby-{i}.jpg", abyFolder);

            var catsTag = await ctx.AddTagAsync("Felines");
            var kittyTag = await ctx.AddTagAsync("Kitty", TagType.Person);

            await ctx.TagFolderAsync(catsFolder, catsTag);

            await ctx.TagPhotoAsync(cat, kittyTag, TagBindingType.Depicted, "0.2,0.2,0.6,0.7");

            var catsPhoto = await ctx.AddPhotoAsync("kittens-1.jpg", catsFolder, descr: "Three kittens on a sleeping mat", date: "2020.10.??");
            await ctx.TagPhotoAsync(catsPhoto, await ctx.AddTagAsync("Alpha", TagType.Person), TagBindingType.Depicted, Calc(720, 450, 58, 38, 233, 205));
            await ctx.TagPhotoAsync(catsPhoto, await ctx.AddTagAsync("Bravo", TagType.Person), TagBindingType.Depicted, Calc(720, 450, 267, 25, 435, 192));
            await ctx.TagPhotoAsync(catsPhoto, await ctx.AddTagAsync("Charlie", TagType.Person), TagBindingType.Depicted, Calc(720, 450, 385, 150, 585, 332));
            await ctx.TagPhotoAsync(catsPhoto, await ctx.AddTagAsync("Mutts"));

            await ctx.AddSharedLink(catsFolder, key: "all-cats", mode: SearchScope.CurrentFolderAndSubfolders);
        }

        /// <summary>
        /// Creates folders with travel-related pictures.
        /// </summary>
        private static async Task SeedTravelFoldersAsync(SeedContext ctx)
        {
            var travelFolder = await ctx.AddFolderAsync("Travel", "travel");

            var tp1 = await ctx.AddPhotoAsync("travel-1.jpg", travelFolder, "Rodos Island", "2018.09.01");
            var tp2 = await ctx.AddPhotoAsync("travel-2.jpg", travelFolder, "Simi Island", "2018.09.04");
            var tp3 = await ctx.AddPhotoAsync("travel-3.jpg", travelFolder, "Colosseum", "2018.04.22");
            var tp4 = await ctx.AddPhotoAsync("travel-4.jpg", travelFolder, "Trevi Fountain", "2018.04.23");
            var tp5 = await ctx.AddPhotoAsync("travel-5.jpg", travelFolder, "Venice", "2018.04.??");

            var greeceTag = await ctx.AddTagAsync("Greece");
            var italyTag = await ctx.AddTagAsync("Italy");
            var sightTag = await ctx.AddTagAsync("Sight-seeing");

            await ctx.TagPhotoAsync(tp1, greeceTag);
            await ctx.TagPhotoAsync(tp1, sightTag);
            await ctx.TagPhotoAsync(tp2, greeceTag);
            await ctx.TagPhotoAsync(tp3, italyTag);
            await ctx.TagPhotoAsync(tp3, sightTag);
            await ctx.TagPhotoAsync(tp4, italyTag);
            await ctx.TagPhotoAsync(tp5, italyTag);

            await ctx.AddSharedLink(travelFolder, new[] {greeceTag.Id}, key: "travel-greece");
        }

        /// <summary>
        /// Creates uncommon folders to test various edge cases.
        /// </summary>
        private static async Task SeedEdgeCaseFoldersAsync(SeedContext ctx)
        {
            var longFolder = await ctx.AddFolderAsync("Z Long folder", "long");
            for (var i = 1; i <= 50; i++)
                await ctx.AddFolderAsync("Subfolder " + i, "sub" + i, longFolder);

            var deepFolder = await ctx.AddFolderAsync("Z Deep folder", "deep");
            var curr = deepFolder;
            for (var i = 1; i <= 10; i++)
                curr = await ctx.AddFolderAsync("Subfolder " + i, "sub" + i, curr);
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
        public static async Task ClearPreviousDataAsync()
        {
            var dir = Directory.GetCurrentDirectory();
            var mediaPath = Path.Combine(dir, "App_Data", "@media");
            if (Directory.Exists(mediaPath))
            {
                await Task.Run(() => Directory.Delete(mediaPath, true));
                Directory.CreateDirectory(mediaPath);
            }

            var dataPath = Path.Combine(dir, "App_Data", "Isotope.db");
            if(File.Exists(dataPath))
                File.Delete(dataPath);
        }
    }
}