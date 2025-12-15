import { ApiClientBase } from "./ApiClientBase";
import { AuthService } from "@common/source/services/AuthService";
import { User } from "../vms/User";
import { UserCreation } from "../vms/UserCreation";
import { UserPassword } from "../vms/UserPassword";

export class UserApiClient extends ApiClientBase {
    constructor($host: string, $auth: AuthService) {
        super($host, $auth, 'users');
    }

    async getList() {
        return this.restGet<User[]>();
    }

    async create(value: UserCreation) {
        return this.restPost<User>(value);
    }

    async update(id: string, value: User) {
        return this.restPut(id, value);
    }

    async updatePassword(id: string, value: UserPassword) {
        return this.restPut(id + '/password', value);
    }

    async remove(id: string) {
        return this.restDelete(id);
    }
}