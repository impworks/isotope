import { ApiClientBase } from "./ApiClientBase";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { FolderTitle } from "../vms/FolderTitle";
import { Folder } from "../vms/Folder";

export class FolderApiClient extends ApiClientBase {
    constructor($host: string, $auth: AuthService) {
        super($host, $auth, 'folders');
    }
    
    async getTree(): Promise<FolderTitle[]> {
        return this.restGet<FolderTitle[]>();
    }
    
    async create(key: string, data: Folder): Promise<FolderTitle> {
        return this.restPost<FolderTitle>(data, key)
    }

    async get(key: string): Promise<Folder> {
        return this.restGet<Folder>(key);
    }
    
    async update(key: string, data: Folder): Promise<void> {
        return this.restPut(key, data);
    }
    
    async remove(key: string): Promise<void> {
        return this.restDelete(key);
    }
}