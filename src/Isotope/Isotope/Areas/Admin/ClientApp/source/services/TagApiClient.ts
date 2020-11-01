import { ApiClientBase } from "./ApiClientBase";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { Tag } from "../vms/Tag";

export class TagApiClient extends ApiClientBase {
    constructor($host: string, $auth: AuthService) {
        super($host, $auth, 'tags');
    }
    
    async get() {
        return this.restGet<Tag[]>();
    }
    
    async create(value: Tag) {
        return this.restPost<Tag>(value);
    }
    
    async update(id: number, value: Tag) {
        return this.restPut(id.toString(), value);
    }
    
    async remove(id: number): Promise<void> {
        return this.restDelete(id.toString());
    }
}