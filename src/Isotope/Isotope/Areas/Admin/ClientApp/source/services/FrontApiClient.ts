import axios, { AxiosRequestConfig } from "axios";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { GalleryInfo } from "../../../../Front/ClientApp/source/vms/GalleryInfo";
import { LoginRequest } from "../../../../Common/source/vms/LoginRequest";
import { LoginResponse } from "../../../../Common/source/vms/LoginResponse";

export class FrontApiClient {
    constructor(private $host: string, private $auth: AuthService) {
    }

    /**
     * Returns the gallery info.
     */
    async getInfo(): Promise<GalleryInfo> {
        const response = await axios.get<GalleryInfo>(this.getUrl('info'), this.getCfg());
        return response.data;
    }

    /**
     * Authorizes the user with given credentials.
     */
    async authorize(creds: LoginRequest): Promise<LoginResponse> {
        const response = await axios.post<LoginResponse>(this.getUrl('auth/login'), creds);
        return response.data;
    }

    /**
     * Returns the request url.
     */
    protected getUrl(key: string): string {
        return this.$host + '/@api/' + key;
    }

    /**
     * Adds authorization header.
     */
    protected getCfg(): AxiosRequestConfig {
        return this.$auth.user
            ? {
                headers: {
                    Authorization: 'Bearer ' + this.$auth.user.token
                }
            }
            : { };
    }
}