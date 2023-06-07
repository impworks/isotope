export interface FolderTitle {
    key: string;
    depth: number;
    caption: string;
    mediaCount: number;
    thumbnailPath: string;
    path: string;
    
    subfolders: FolderTitle[];
}