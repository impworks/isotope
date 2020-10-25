import { SearchScope as SearchMode } from "./SearchScope";

export interface FolderContentsRequest {
    folder: string;
    tags?: string;
    searchMode?: SearchMode;
    dateFrom?: string;
    dateTo?: string;
}