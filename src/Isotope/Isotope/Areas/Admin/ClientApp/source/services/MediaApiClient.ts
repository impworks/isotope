import axios from 'axios';
import { ApiClientBase } from "./ApiClientBase";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { MediaListRequest } from "../vms/MediaListRequest";
import { MediaThumbnail } from "../vms/MediaThumbnail";
import { Media } from "../vms/Media";
import { Rect } from "../../../../Common/source/vms/Rect";

export class MediaApiClient extends ApiClientBase {
    constructor($host: string, $auth: AuthService) {
        super($host, $auth, 'media');
    }
    
    async getList(req: MediaListRequest) {
        return this.restGet<MediaThumbnail[]>(null, req);
    }
    
    async upload(folderKey: string, file: File) {
        const url = this.getUrl(null, { folder: folderKey });
        
        const config = this.getCfg();
        config.headers['Content-Type'] = 'multipart/form-data';
        
        const form = new FormData();
        form.append('file', file);
        
        const response = await axios.post<MediaThumbnail>(url, form);
        return response.data;
    }

    async get(key: string) {
        return this.restGet<Media>(key);
    }
    
    async update(key: string, value: Media) {
        return this.restPut(key, value);
    }

    async getThumb(key: string) {
        return this.restGet<Rect>(key + '/thumb');
    }

    async updateThumb(key: string, value: Rect) {
        return this.restPut(key + '/thumb', value);
    }
    
    async remove(key: string) {
        return this.restDelete(key);
    }
}