import { ConfigApiClient } from "./ConfigApiClient";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { SharedLinkApiClient } from "./SharedLinkApiClient";
import { TagApiClient } from "./TagApiClient";
import { UserApiClient } from "./UserApiClient";
import { FolderApiClient } from "./FolderApiClient";
import { MediaApiClient } from "./MediaApiClient";
import { FrontApiClient } from "./FrontApiClient";

/**
 * Unified access point to all admin API methods.
 */
export class ApiService {
    constructor($host: string, $auth: AuthService) {
        this.front = new FrontApiClient($host, $auth);
        
        this.folders = new FolderApiClient($host, $auth);
        this.media = new MediaApiClient($host, $auth);
        this.tags = new TagApiClient($host, $auth);
        
        this.users = new UserApiClient($host, $auth);
            
        this.config = new ConfigApiClient($host, $auth);
        this.sharedLinks = new SharedLinkApiClient($host, $auth);
    }
    
    readonly front: FrontApiClient;

    readonly folders: FolderApiClient;
    readonly media: MediaApiClient;
    readonly tags: TagApiClient;

    readonly users: UserApiClient;
    
    readonly config: ConfigApiClient;
    readonly sharedLinks: SharedLinkApiClient;    
}