import { TagBinding } from "./TagBinding";
import { OverlayTagBinding } from "./OverlayTagBinding";

export interface Media {
    fullPath: string;
    description: string;
    date: string;
    extraTags: TagBinding[];
    overlayTags: OverlayTagBinding[];
}