import { ApiClientBase } from "./ApiClientBase";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { GalleryInfo } from "../../../../Front/ClientApp/source/vms/GalleryInfo";

export class InfoApiClient extends ApiClientBase {
    constructor($host: string, $auth: AuthService) {
        super($host, $auth, null);
    }

    getInfo(): Promise<GalleryInfo> {
        return this.restGet(); 
    }
    
    protected getUrl(key?: string, query?: any): string {
        return this.$host + '/@api/info';
    }
}