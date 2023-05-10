import { SearchCriteria } from "@asf/ng14-library";

export class CategoryAssociationWrite {
    public name: string;
    public fiscalCode: string;
    public email: string;
    public telephoneNumber: string;
}
export class CategoryAssociation extends CategoryAssociationWrite {
    public id: number;
    public isDeleted: boolean;
}


export class CategoryAssociationSearchCriteria extends SearchCriteria {
    public name: string;
    public fiscalCode: string;
}
