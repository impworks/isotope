import axios from 'axios';
import { AuthService } from "../../../../Common/source/services/AuthService";
import { StaticHelper } from "../../../../Common/source/utils/StaticHelper";
import { AxiosRequestConfig } from "axios";

/**
 * Base class for API clients for each data section.
 */
export class ApiClientBase {
    constructor(
        protected $host: string,
        protected $auth: AuthService,
        section: string
    ) {
        this._section = section;
    }
    
    protected _section: string;

    /**
     * Sends a GET request.
     */
    protected async restGet<T>(key?: string, query?: any): Promise<T> {
        const url = this.getUrl(key, query);
        const cfg = this.getCfg();
        return (await axios.get<T>(url, cfg)).data;
    }

    /**
     * Sends a POST request.
     */
    protected async restPost<T>(body: any, key?: string, query?: any): Promise<T> {
        const url = this.getUrl(key, query);
        const cfg = this.getCfg();
        return (await axios.post<T>(url, body, cfg)).data;
    }

    /**
     * Sends a PUT request.
     */
    protected async restPut<T>(key: string, body: any, query?: any): Promise<T> {
        const url = this.getUrl(key, query);
        const cfg = this.getCfg();
        return (await axios.put<T>(url, body, cfg)).data;
    }

    /**
     * Sends a DELETE request.
     */
    protected async restDelete(key: string, query?: any): Promise<void> {
        const url = this.getUrl(key, query);
        const cfg = this.getCfg();
        await axios.delete(url, cfg);
    }
    
    /**
     * Returns the URL for invoking a method.
     */
    protected getUrl(key?: string, query?: any): string {
        let path = this.$host + '/@api/admin/' + this._section;
        if(key)
            path += '/' + key;
        if(query)
            path += '?' + StaticHelper.getQuery(query);
        return path;
    }

    /**
     * Adds authorization header.
     */
    protected getCfg(): AxiosRequestConfig {
        return {
            headers: {
                Authorization: 'Bearer ' + this.$auth.user.token
            } 
        };
    }
}