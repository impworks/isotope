import { SharedLink } from "./SharedLink";

export interface SharedLinkDetails extends SharedLink {
    creationDate: string;
    key: string;
    folderCaption: string;
    tagCount: number;
}