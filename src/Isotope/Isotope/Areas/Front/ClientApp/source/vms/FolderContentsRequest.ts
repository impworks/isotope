import { SearchMode } from "./SearchMode";

export interface FolderContentsRequest {
    folder: string;
    tags?: string;
    searchMode?: SearchMode;
    dateFrom?: string;
    dateTo?: string;
}