import { TagBindingWithLocation, TagBindingWithType } from "./TagBinding";
import { MediaType } from "../../../../Common/source/vms/MediaType";
import { Folder } from "../../../Admin/ClientApp/source/vms/Folder";

export interface Media {
    key: string;
    type: MediaType;
    fullPath: string;
    originalPath: string;
    date: string;
    description: string;
    width?: number;
    height?: number;

    // EXIF metadata
    cameraMake?: string;
    cameraModel?: string;
    lensModel?: string;
    exposureTime?: string;
    fNumber?: string;
    isoSpeed?: number;
    focalLength?: string;
    latitude?: number;
    longitude?: number;

    overlayTags: TagBindingWithLocation[];
    extraTags: TagBindingWithType[];
    folder?: Folder;
}