import { MediaType } from "./MediaType";
import { TagBindingWithLocation, TagBindingWithType } from "./TagBinding";

export interface Media {
    key: string;
    type: MediaType;
    fullPath: string;
    originalPath: string;
    date: string;
    description: string;
    overlayTags: TagBindingWithLocation[];
    extraTags: TagBindingWithType[];
}