import { ConfigApiClient } from "./ConfigApiClient";
import { AuthService } from "../../../../Common/source/services/AuthService";

export class ApiService {
    constructor($host: string, $auth: AuthService) {
        this.config = new ConfigApiClient($host, $auth);
    }
    
    readonly config: ConfigApiClient;
}