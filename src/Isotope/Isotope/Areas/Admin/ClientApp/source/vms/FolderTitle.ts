export interface FolderTitle {
    key: string;
    caption: string;
    mediaCount: number;
    
    subfolders: FolderTitle[];
}