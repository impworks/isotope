import axios, { AxiosRequestConfig } from 'axios';
import { Folder } from "../vms/Folder";
import { FolderContentsRequest } from "../vms/FolderContentsRequest";
import { FolderContents } from "../vms/FolderContents";
import { Tag } from "../vms/Tag";
import { Media } from "../vms/Media";
import { GalleryInfo } from "../vms/GalleryInfo";
import { FilterStateService, IFilterState } from "./FilterStateService";
import { KeyResult } from "../../../../Common/source/vms/KeyResult";
import { ILookup } from "../../../../Common/source/utils/Interfaces";
import { StaticHelper } from "../../../../Common/source/utils/StaticHelper";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { LoginRequest } from '../../../../Common/source/vms/LoginRequest';
import { LoginResponse } from "../../../../Common/source/vms/LoginResponse";
import { ChangePasswordRequest } from "../../../../Common/source/vms/ChangePasswordRequest";

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
        $auth.onUserChanged.subscribe(() => this._cache = {});
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
     * Creates a new shared link.
     */
    async createSharedLink(caption: string, state: IFilterState): Promise<string> {
        const url = this.getRequestUrl('admin/shared-links');
        const cfg = this.getRequestConfig();
        const response = await axios.post<KeyResult>(url, { ...state, caption }, cfg);
        return response.data.key;
    }

    /**
     * Performs authorization.
     */
    async authorize(request: LoginRequest): Promise<LoginResponse> {
        const url = this.getRequestUrl('auth/login');
        const response = await axios.post<LoginResponse>(url, request);
        return response.data;
    }

    /**
     * Changes the password.
     */
    async changePassword(request: ChangePasswordRequest): Promise<any> {
        const url = this.getRequestUrl('auth/change-password');
        const cfg = this.getRequestConfig();
        return await axios.post<any>(url, request, cfg);
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