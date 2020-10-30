import { SearchScope } from "../../../../Common/source/vms/SearchScope";

export interface FolderContentsRequest {
    folder: string;
    tags?: string;
    scope?: SearchScope;
    from?: string;
    to?: string;
}