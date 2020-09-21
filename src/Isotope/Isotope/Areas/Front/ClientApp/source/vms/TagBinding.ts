import { Tag } from "./Tag";
import { TagBindingType } from "./TagBindingType";
import { TagBindingLocation } from "./TagBindingLocation";

export interface TagBinding {
    tag: Tag;
    location: TagBindingLocation;
    type: TagBindingType;
}