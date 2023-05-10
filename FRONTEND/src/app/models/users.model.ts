import { SearchCriteria } from "@asf/ng14-library";
import { PermissionCodes } from "./common.model";

export class UserWrite {
    public email: string;
    public firstName: string;
    public lastName: string;
    public fiscalCode: string;
    public phoneNumber: string;
    public isEnabled: boolean;
    public avatarId: string;
    public permissionCode: PermissionCodes;
    public driverId: number;
}

export class User extends UserWrite {
    public id: string;
    public avatarUrl: string;
    public isSpidUser: boolean;
}

export class UserSearchCriteria extends SearchCriteria {
    public fullTextSearch: string;
    public permissionCode: string;
    public smartPaUserId: string;
    public isEnabled: boolean;
}
