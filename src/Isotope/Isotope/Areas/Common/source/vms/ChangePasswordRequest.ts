export interface ChangePasswordRequest {
    oldPassword: string;
    newPassword: string;
    newPasswordRepeat: string;
}