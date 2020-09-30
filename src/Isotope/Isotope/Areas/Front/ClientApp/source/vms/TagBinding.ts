import { Tag } from "./Tag";
import { TagBindingType } from "./TagBindingType";
import { TagBindingLocation } from "./TagBindingLocation";

export interface TagBinding {
    id: number;
    tag: Tag;
}

export interface TagBindingWithType extends TagBinding {
    type: TagBindingType;
}

export interface TagBindingWithLocation extends TagBindingWithType {
    location: TagBindingLocation;
}