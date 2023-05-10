import { SearchCriteria } from "@asf/ng14-library";

export class ProtocolEmail {
    public id: number;
    public description: string;
    public active: boolean;
    public email: string;
}

export class ProtocolEmailSearchCriteria extends SearchCriteria {
    public description: string;
    public active: boolean;
    public email: string;
}