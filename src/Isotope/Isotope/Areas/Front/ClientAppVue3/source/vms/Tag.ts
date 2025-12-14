import { TagType } from "../../../../Common/source/vms/TagType";

export interface Tag {
    id: number;
    caption: string;
    type: TagType;
}