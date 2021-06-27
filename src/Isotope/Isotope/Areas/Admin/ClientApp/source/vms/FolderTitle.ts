export interface FolderTitle {
    key: string;
    depth: number;
    caption: string;
    mediaCount: number;
    thumbnailPath: string;
    
    subfolders: FolderTitle[];
}