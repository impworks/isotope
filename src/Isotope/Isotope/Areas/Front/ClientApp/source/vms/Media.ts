import { TagBindingWithLocation, TagBindingWithType } from "./TagBinding";
import { MediaType } from "../../../../Common/source/vms/MediaType";
import { Folder } from "../../../../Admin/ClientApp/source/vms/Folder";

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
    folder?: Folder;
}