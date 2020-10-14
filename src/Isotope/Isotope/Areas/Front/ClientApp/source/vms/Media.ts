import { MediaType } from "./MediaType";
import { TagBindingWithLocation, TagBindingWithType } from "./TagBinding";

export interface Media {
    key: string;
    type: MediaType;
    fullPath: string;
    originalPath: string;
    date: string;
    description: string;
    width?: number;
    height?: number;
    overlayTags: TagBindingWithLocation[];
    extraTags: TagBindingWithType[];
}