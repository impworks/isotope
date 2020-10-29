import { Tag } from "./Tag";
import { TagBindingType } from "../../../../Common/source/vms/TagBindingType";
import { Rect } from "../../../../Common/source/vms/Rect";

export interface TagBinding {
    id: number;
    tag: Tag;
}

export interface TagBindingWithType extends TagBinding {
    type: TagBindingType;
}

export interface TagBindingWithLocation extends TagBindingWithType {
    location: Rect;
}