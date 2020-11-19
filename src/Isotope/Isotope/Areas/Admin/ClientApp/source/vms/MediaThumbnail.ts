import { MediaType } from "../../../../Common/source/vms/MediaType";

export interface MediaThumbnail {
    key: string;
    type: MediaType;
    thumbnailPath: string;
    uploadDate: string;
    tags: number;
    nonce: number;
}