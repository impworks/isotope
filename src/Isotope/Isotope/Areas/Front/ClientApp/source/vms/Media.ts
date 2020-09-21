import { MediaType } from "./MediaType";
import { TagBinding } from "./TagBinding";

export interface Media {
    key: string;
    type: MediaType;
    fullPath: string;
    originalPath: string;
    date: string;
    description: string;
    tags: TagBinding[];
}