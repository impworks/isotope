import { ApiClientBase } from "./ApiClientBase";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { FolderTitle } from "../vms/FolderTitle";
import { Folder } from "../vms/Folder";

export class FolderApiClient extends ApiClientBase {
    constructor($host: string, $auth: AuthService) {
        super($host, $auth, 'folders');
    }
    
    async getTree(rootKey: string = null) {
        return this.restGet<FolderTitle[]>(null, { rootKey });
    }
    
    async create(key: string, data: Folder) {
        return this.restPost<FolderTitle>(data, key)
    }

    async get(key: string) {
        return this.restGet<Folder>(key);
    }
    
    async update(key: string, data: Folder) {
        return this.restPut(key, data);
    }
    
    async remove(key: string) {
        return this.restDelete(key);
    }
    
    async move(key: string, target: string) {
        return this.restPost({ sourceKey: key, targetKey: target }, 'move');
    }
}