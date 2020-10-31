import { ApiClientBase } from "./ApiClientBase";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { User } from "../vms/User";
import { UserCreation } from "../vms/UserCreation";
import { UserPassword } from "../vms/UserPassword";

export class UserApiClient extends ApiClientBase {
    constructor($host: string, $auth: AuthService) {
        super($host, $auth, 'users');
    }

    async get(): Promise<User[]> {
        return this.restGet<User[]>();
    }

    async create(value: UserCreation): Promise<User> {
        return this.restPost<User>(value);
    }

    async update(id: string, value: User): Promise<void> {
        return this.restPut(id, value);
    }

    async updatePassword(id: string, value: UserPassword): Promise<void> {
        return this.restPut(id + '/password', value);
    }

    async remove(id: string): Promise<void> {
        return this.restDelete(id);
    }
}