import { SearchCriteria } from "@asf/ng14-library";
import { TaxiDriverDocument } from "./licensees.model";


export class PersonBase {
    public phoneNumber: string;
    public emailOrPEC: string;
    public address: string;
    public zipCode: string;
    public residentCity: string;
    public residentProvince: string;
}

export class PersonWrite extends PersonBase {
    public firstName: string;
    public lastName: string;
    public fiscalCode: string;
    public birthDate: string;
    public birthCity: string;
    public birthProvince: string;
}
export class PersonItem extends PersonBase {
    public id: number;
    public displayName: string;
    public fiscalCode: string;
}

export class Person extends PersonWrite {
    public id: number;
    public type: PersonTypes;
}

export enum PersonTypes {
    Physical = "Physical",
    Legal = "Legal"
}

export class PersonAutocomplete {
    public id: number;
    public displayName: string;
}

export class PeopleSearchCriteria extends SearchCriteria {
    public licenseeId: number;
    public licenseeNumber: string;
    public personDescription: string;
    public vehicleId: number;
    public fiscalCode: string;
    public phoneNumber: string;
    public emailOrPec: string;
    public personType: PersonTypes = null;
}
