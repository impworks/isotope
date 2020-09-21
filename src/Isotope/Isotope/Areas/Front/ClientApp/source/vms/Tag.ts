import { TagType } from "./TagType";

export interface Tag {
    id: number;
    caption: string;
    type: TagType;
}