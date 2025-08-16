import { TagBinding } from "./TagBinding";
import { MediaThumbnail } from "./MediaThumbnail";
import { Folder } from "./Folder";

export interface FolderContents {
    thumbnailUrl?: string;
    tags: TagBinding[];
    subfolders: Folder[];
    media: MediaThumbnail[];
}