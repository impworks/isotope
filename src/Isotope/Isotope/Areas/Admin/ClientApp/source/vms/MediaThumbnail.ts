import { MediaType } from "../../../../Front/ClientApp/source/vms/MediaType";

export interface MediaThumbnail {
    key: string;
    type: MediaType;
    thumbnailPath: string;
    uploadDate: string;
    tags: number;
}