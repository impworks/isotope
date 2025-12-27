export interface Stats {
    folderCount: number;
    photoCount: number;
    tagCount: number;
    tagBindingCount: number;
    sharedLinkCount: number;

    databaseSizeBytes: number;
    originalPhotosSizeBytes: number;
    imageCacheSizeBytes: number;

    usedMemoryBytes: number;
    availableMemoryBytes: number;
    dotNetVersion: string;
    operatingSystem: string;
    buildCommit: string;
}
