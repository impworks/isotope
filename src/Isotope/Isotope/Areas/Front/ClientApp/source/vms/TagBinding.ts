import { Tag } from "./Tag";
import { TagBindingType } from "./TagBindingType";
import { TagBindingLocation } from "./TagBindingLocation";

export interface TagBinding {
    tag: Tag;
}

export interface TagBindingWithType extends TagBinding {
    type: TagBindingType;
}

export interface TagBindingWithLocation extends TagBindingWithType {
    location: TagBindingLocation;
}