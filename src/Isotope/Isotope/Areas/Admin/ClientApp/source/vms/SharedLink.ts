import { SearchScope } from "../../../../Front/ClientApp/source/vms/SearchScope";

export interface SharedLink {
    folder: string;
    tags: number[];
    from: string;
    to: string;
    scope: SearchScope;
}