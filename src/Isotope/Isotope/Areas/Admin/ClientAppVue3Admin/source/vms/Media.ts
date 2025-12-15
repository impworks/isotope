import { TagBinding } from "./TagBinding";
import { OverlayTagBinding } from "./OverlayTagBinding";

export interface Media {
    thumbnailPath: string;
    fullPath: string;
    description: string;
    date: string;
    extraTags: TagBinding[];
    overlayTags: OverlayTagBinding[];
}