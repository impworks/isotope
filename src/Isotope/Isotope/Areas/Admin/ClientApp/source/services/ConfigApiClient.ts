import { ApiClientBase } from "./ApiClientBase";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { Config } from "../vms/Config";

export class ConfigApiClient extends ApiClientBase {
    constructor($host: string, $auth: AuthService) {
        super($host, $auth);        
    }
    
    async get(): Promise<Config> {
        return this.invoke<Config>({ verb: 'get', method: 'config' });
    }

    async set(value: Config) {
        return this.invoke({ verb: 'put', method: 'config', data: value });
    }
}