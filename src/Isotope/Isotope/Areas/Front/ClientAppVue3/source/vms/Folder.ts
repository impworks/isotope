export interface Folder {
    caption: string;
    path: string;
    thumbnailPath: string;
    mediaCount: string;
    subfolders: Folder[];
}