import axios, { AxiosRequestConfig } from 'axios';
import { AuthService } from "./AuthService";
import { Folder } from "../vms/Folder";
import { StaticHelper } from "../utils/StaticHelper";
import { FolderContentsRequest } from "../vms/FolderContentsRequest";
import { FolderContents } from "../vms/FolderContents";
import { Tag } from "../vms/Tag";
import { Media } from "../vms/Media";
import { LoginRequest } from "../vms/LoginRequest";
import { LoginResponse } from "../vms/LoginResponse";
import { GalleryInfo } from "../vms/GalleryInfo";
import { FilterStateService } from "./FilterStateService";
import { ILookup } from "../utils/Interfaces";

export class ApiService {
    // -----------------------------------
    // Constructor
    // -----------------------------------
    
    public constructor(
        private $host: string,
        private $auth: AuthService,
        private $filter: FilterStateService
    ) {
        this._cache = {};
    }
    
    private _cache: ILookup<Promise<any>>;

    // -----------------------------------
    // Public methods
    // -----------------------------------

    /**
     * Returns the information about the gallery.
     */
    getInfo(): Promise<GalleryInfo> {
        return this.invokeCached<GalleryInfo>('info');
    }

    /**
     * Returns the folder tree.
     */
    getFolderTree(): Promise<Folder[]> {
        return this.invokeCached<Folder[]>('tree');
    }

    /**
     * Returns the contents of a folder (with or without filtering).
     */
    getFolderContents(request: FolderContentsRequest): Promise<FolderContents> {
        return this.invoke<FolderContents>('folder', request);
    }

    /**
     * Returns the list of known tags.
     */
    getTags(): Promise<Tag[]> {
        return this.invokeCached<Tag[]>('tags');
    }

    /**
     * Returns the details of a media file.
     */
    getMedia(key: string): Promise<Media> {
        return this.invoke<Media>('media', { key: key });
    }

    /**
     * Performs authorization.
     */
    async authorize(request: LoginRequest): Promise<LoginResponse> {
        const url = this.getRequestUrl('auth/login');
        const response = await axios.post<LoginResponse>(url, request);
        return response.data;
    }

    // -----------------------------------
    // Private helpers
    // -----------------------------------

    /**
     * Sends an API request to the specified method.
     */
    private async invoke<T>(method: string, query?: any): Promise<T> {
        const url = this.getRequestUrl(method, query);
        const cfg = this.getRequestConfig();
        const response = await axios.get<T>(url, cfg);
        return response.data;
    }

    /**
     * Returns the authorization config.
     */
    private getRequestConfig(): AxiosRequestConfig {
        const user = this.$auth.user;
        return user
            ? { headers: { Authorization: 'Bearer ' + user.token } }
            : { };
    }

    /**
     * Builds the API method URL using name and arguments.
     */
    private getRequestUrl(method: string, query?: any) {
        return this.$host + '/@api/' + method + '?' + StaticHelper.getQuery(query, { sh: this.$filter.shareId });
    }

    /**
     * Memoizes the first call, reusing the existing promise for later calls.
     */
    private invokeCached<T>(method: string): Promise<T> {
        const result = this._cache[method];
        return result ? result : (this._cache[method] = this.invoke<T>(method));
    }
}