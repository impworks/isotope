import { ConfigApiClient } from "./ConfigApiClient";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { SharedLinkApiClient } from "./SharedLinkApiClient";
import { TagApiClient } from "./TagApiClient";
import { UserApiClient } from "./UserApiClient";

export class ApiService {
    constructor($host: string, $auth: AuthService) {
        this.tags = new TagApiClient($host, $auth);
        
        this.users = new UserApiClient($host, $auth);
            
        this.config = new ConfigApiClient($host, $auth);
        this.sharedLinks = new SharedLinkApiClient($host, $auth);
    }

    readonly tags: TagApiClient;

    readonly users: UserApiClient;
    
    readonly config: ConfigApiClient;
    readonly sharedLinks: SharedLinkApiClient;
}