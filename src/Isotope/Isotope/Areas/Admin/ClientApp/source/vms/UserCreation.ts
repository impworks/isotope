import { User } from "./User";
import { UserPassword } from "./UserPassword";

export interface UserCreation {
    userInfo: User;
    passwordInfo: UserPassword;
}