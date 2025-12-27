import { ApiClientBase } from "./ApiClientBase";
import { AuthService } from "@common/source/services/AuthService";
import type { Stats } from "@/vms/Stats";

export class StatsApiClient extends ApiClientBase {
    constructor($host: string, $auth: AuthService) {
        super($host, $auth, 'stats');
    }

    async get() {
        return this.restGet<Stats>();
    }
}
