import { ApiClientBase } from "./ApiClientBase";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { SharedLinkDetails } from "../vms/SharedLinkDetails";

export class SharedLinkApiClient extends ApiClientBase {
    constructor($host: string, $auth: AuthService) {
        super($host, $auth, 'shared-links');
    }
    
    async getList() {
        return this.restGet<SharedLinkDetails[]>();
    }
    
    async remove(key: string) {
        return this.restDelete(key);
    }
}