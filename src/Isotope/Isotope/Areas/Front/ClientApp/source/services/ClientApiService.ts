import axios, { AxiosRequestConfig } from 'axios';
import { UserStateService } from "./UserStateService";
import { Folder } from "../vms/Folder";
import { StaticHelper } from "../utils/StaticHelper";
import { FolderContentsRequest } from "../vms/FolderContentsRequest";
import { FolderContents } from "../vms/FolderContents";
import { Tag } from "../vms/Tag";
import { Media } from "../vms/Media";

export class ClientApiService {
    // -----------------------------------
    // Constructor
    // -----------------------------------
    
    public constructor(
        private $apiHost: string,
        private $userState: UserStateService
    ) {
    }

    // -----------------------------------
    // Public methods
    // -----------------------------------

    /**
     * Returns the folder tree.
     */
    async getFolderTree(): Promise<Folder[]> {
        return await this.invoke<Folder[]>('tree');
    }

    /**
     * Returns the contents of a folder (with or without filtering).
     */
    async getFolderContents(request: FolderContentsRequest): Promise<FolderContents> {
        return await this.invoke<FolderContents>('folder', request);
    }

    /**
     * Returns the list of known tags.
     */
    async getTags(): Promise<Tag[]> {
        return await this.invoke<Tag[]>('tags');
    }

    /**
     * Returns the details of a media file.
     */
    async getMedia(key: string): Promise<Media> {
        return await this.invoke<Media>('media', { key: key });
    }
    

    // -----------------------------------
    // Private helpers
    // -----------------------------------

    /**
     * Sends an API request to the specified method.
     */
    private async invoke<T>(method: string, query?: any): Promise<T> {
        const url = this.$apiHost + '/@api/' + method + '?' + StaticHelper.getQuery(query, { sh: this.$userState.shareId });
        const cfg = this.getRequestConfig();
        const response = await axios.get<T>(url, cfg);
        return response.data;
    }

    /**
     * Returns the authorization config.
     */
    private getRequestConfig(): AxiosRequestConfig {
        const user = this.$userState.user;
        return user
            ? { headers: { authorization: 'Bearer ' + user.token } }
            : { };
    }
}