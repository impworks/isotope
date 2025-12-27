namespace Isotope.Areas.Admin.Dto;

/// <summary>
/// Gallery statistics view model.
/// </summary>
public class StatsVM
{
    public int FolderCount { get; set; }
    public int PhotoCount { get; set; }
    public int TagCount { get; set; }
    public int TagBindingCount { get; set; }
    public int SharedLinkCount { get; set; }

    public long DatabaseSizeBytes { get; set; }
    public long OriginalPhotosSizeBytes { get; set; }
    public long ImageCacheSizeBytes { get; set; }

    public long UsedMemoryBytes { get; set; }
    public long AvailableMemoryBytes { get; set; }
    public string DotNetVersion { get; set; }
    public string OperatingSystem { get; set; }
    public string BuildCommit { get; set; }
}
