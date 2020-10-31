import axios from 'axios';
import { AuthService } from "../../../../Common/source/services/AuthService";
import { StaticHelper } from "../../../../Common/source/utils/StaticHelper";
import { AxiosRequestConfig } from "axios";

export type HttpVerb = 'get' | 'post' | 'put' | 'delete';
export type Request = {
    verb: HttpVerb;
    method: string;
    query?: any;
    data?: any;
}

/**
 * Base class for API clients for each data section.
 */
export class ApiClientBase {
    constructor(
        protected $host: string,
        protected $auth: AuthService
    ) {
        
    }

    /**
     * Invokes the HTTP api.
     */
    protected async invoke<T>(req: Request): Promise<T> {
        const url = this.getUrl(req.method, req.query);
        const cfg = this.getCfg();
        
        if(req.verb === 'get')
            return (await axios.get<T>(url, cfg)).data;
        
        if(req.verb === 'post')
            return (await axios.post<T>(url, req.data, cfg)).data;
            
        if(req.verb === 'put')
            return (await axios.put<T>(url, req.data, cfg)).data;
        
        if(req.verb === 'delete')
            return (await axios.delete(url, cfg)).data;
        
        throw Error('Unsupported http verb: ' + req.verb);
    }

    /**
     * Returns the URL for invoking a method.
     */
    protected getUrl(method: string, query?: any): string {
        let path = this.$host + '/@api/admin/' + method;
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