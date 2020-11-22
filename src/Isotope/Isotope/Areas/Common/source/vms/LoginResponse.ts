import { UserInfo } from "./UserInfo";

export interface LoginResponse {
    success: boolean;
    errorMessage: string;
    user: UserInfo;
}