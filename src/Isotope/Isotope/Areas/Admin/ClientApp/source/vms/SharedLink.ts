import { SearchScope } from "../../../../Common/source/vms/SearchScope";

export interface SharedLink {
    folder: string;
    tags: number[];
    from: string;
    to: string;
    scope: SearchScope;
}