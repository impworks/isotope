using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Data;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Admin.Services;

/// <summary>
/// Service for retrieving gallery statistics.
/// </summary>
public class StatsManager(AppDbContext db)
{
    /// <summary>
    /// Returns all gallery statistics.
    /// </summary>
    public async Task<StatsVM> GetAsync()
    {
        var (originalSize, cacheSize) = GetMediaSizes();

        return new StatsVM
        {
            FolderCount = await db.Folders.CountAsync() - 1,
            PhotoCount = await db.Media.CountAsync(),
            TagCount = await db.Tags.CountAsync(),
            TagBindingCount = await db.MediaTags.CountAsync() + await db.FolderTags.CountAsync(),
            SharedLinkCount = await db.SharedLinks.CountAsync(),

            DatabaseSizeBytes = await GetDatabaseSizeAsync(),
            OriginalPhotosSizeBytes = originalSize,
            ImageCacheSizeBytes = cacheSize,
            TotalDiskBytes = GetTotalDiskBytes(),

            UsedMemoryBytes = GC.GetTotalMemory(false),
            AvailableMemoryBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes,
            DotNetVersion = RuntimeInformation.FrameworkDescription,
            OperatingSystem = RuntimeInformation.OSDescription,
            BuildCommit = Environment.GetEnvironmentVariable("BuildCommit") ?? "unknown"
        };
    }

    private async Task<long> GetDatabaseSizeAsync()
    {
        var conn = db.Database.GetDbConnection();
        await conn.OpenAsync();
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT page_size * page_count FROM pragma_page_count(), pragma_page_size();";
            var result = await cmd.ExecuteScalarAsync();
            return result is long size ? size : Convert.ToInt64(result);
        }
        finally
        {
            await conn.CloseAsync();
        }
    }

    private static long GetTotalDiskBytes()
    {
        try
        {
            var appDataPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "App_Data"));
            var root = Path.GetPathRoot(appDataPath) ?? "/";
            return new DriveInfo(root).TotalSize;
        }
        catch
        {
            return 0;
        }
    }

    private static (long OriginalSize, long CacheSize) GetMediaSizes()
    {
        var mediaPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "@media");
        if (!Directory.Exists(mediaPath))
            return (0, 0);

        long originalSize = 0;
        long cacheSize = 0;

        foreach (var file in Directory.EnumerateFiles(mediaPath, "*", SearchOption.AllDirectories))
        {
            var size = new FileInfo(file).Length;
            if (file.EndsWith(".lg.jpg", StringComparison.OrdinalIgnoreCase) ||
                file.EndsWith(".sm.jpg", StringComparison.OrdinalIgnoreCase))
            {
                cacheSize += size;
            }
            else
            {
                originalSize += size;
            }
        }

        return (originalSize, cacheSize);
    }
}
