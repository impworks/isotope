import { SharedLink } from "./SharedLink";

export interface SharedLinkDetails extends SharedLink {
    key: string;
    folderCaption: string;
    tagCount: number;
}