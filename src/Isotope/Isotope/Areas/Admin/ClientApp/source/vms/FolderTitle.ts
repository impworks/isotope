export interface FolderTitle {
    key: string;
    depth: number;
    caption: string;
    slug: string;
    mediaCount: number;
    thumbnailPath: string;
    path: string;
    
    collapsed?: boolean;
    hidden?: boolean;
    
    subfolders: FolderTitle[];
}