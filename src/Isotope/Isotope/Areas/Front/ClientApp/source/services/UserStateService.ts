import { UserInfo } from "../vms/UserInfo";

export class UserStateService {
    
    public constructor() {
        this._user = this.getStoredUser();
    }
    
    private static USER_KEY = "isotope-auth-info";
    
    private _user: UserInfo;
    private _shareId: string;
    
    get user(): UserInfo {
        return this._user;
    }
    
    set user(value: UserInfo) {
        this._user = value;
        this.setStoredUser(value);
    }
    
    get shareId(): string {
        return this._shareId;
    }
    
    set shareId(value: string) {
        this._shareId = value;
    }

    /**
     * Retrieves the locally cached user.
     */
    private getStoredUser(): UserInfo {
        const ls = window.localStorage;
        const raw = ls[UserStateService.USER_KEY];
        
        if(!raw)
            return null;
        
        try {
            return JSON.parse(raw);
        } catch (e) {
            ls.removeItem(UserStateService.USER_KEY);
            return null;
        }
    }

    /**
     * Updates the current user authorization.
     */
    private setStoredUser(user: UserInfo) {
        const ls = window.localStorage;
        const raw = JSON.stringify(user);
        ls.setItem(UserStateService.USER_KEY, raw);
    }
}