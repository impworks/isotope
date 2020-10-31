import { ConfigApiClient } from "./ConfigApiClient";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { SharedLinkApiClient } from "./SharedLinkApiClient";

export class ApiService {
    constructor($host: string, $auth: AuthService) {
        this.config = new ConfigApiClient($host, $auth);
        this.sharedLinks = new SharedLinkApiClient($host, $auth);
    }
    
    readonly config: ConfigApiClient;
    readonly sharedLinks: SharedLinkApiClient;
}