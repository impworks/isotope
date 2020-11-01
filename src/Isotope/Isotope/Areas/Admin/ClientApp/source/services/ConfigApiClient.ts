import { ApiClientBase } from "./ApiClientBase";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { Config } from "../vms/Config";

export class ConfigApiClient extends ApiClientBase {
    constructor($host: string, $auth: AuthService) {
        super($host, $auth, 'config');        
    }
    
    async get() {
        return this.restGet<Config>();
    }

    async update(value: Config) {
        return this.restPut(null, value)
    }
}