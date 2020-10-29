import { SearchScope } from "../../../../Common/source/vms/SearchScope";

export interface FolderContentsRequest {
    folder: string;
    tags?: string;
    scope?: SearchScope;
    dateFrom?: string;
    dateTo?: string;
}