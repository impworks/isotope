import { ApiClientBase } from "./ApiClientBase";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { FolderTitle } from "../vms/FolderTitle";
import { Folder } from "../vms/Folder";

export class FolderApiClient extends ApiClientBase {
    constructor($host: string, $auth: AuthService) {
        super($host, $auth, 'folders');
    }
    
    async getTree() {
        return this.restGet<FolderTitle[]>();
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
}