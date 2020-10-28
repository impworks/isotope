import { TagType } from "../../../../Front/ClientApp/source/vms/TagType";

export interface Tag {
    id: number;
    caption: string;
    type: TagType;
}