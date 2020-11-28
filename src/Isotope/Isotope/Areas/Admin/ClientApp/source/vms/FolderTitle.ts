export interface FolderTitle {
    key: string;
    caption: string;
    mediaCount: number;
    thumbnailPath: string;
    
    subfolders: FolderTitle[];
}