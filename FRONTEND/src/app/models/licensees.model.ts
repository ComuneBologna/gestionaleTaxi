import { SearchCriteria } from "@asf/ng14-library";
import { PersonAutocomplete } from "./people.model";

//#region Licensee
export class BaseLicensee {
    public id: number;
    public vehicleId: number;
    public taxiDriverAssociationName: string;
    public endDate: string;
    public licenseesIssuingOfficeDescription: string;
    public licenseesIssuingOfficeId: number;
    public releaseDate: string;
    public activityExpiredOnCause: string;
    public taxiDriverAssociationId: number;
    public note: string;
    public acronym: string;
    public isFamilyCollaboration: boolean = false;
    public number: string;
    public status: LicenseeStateTypes;
    public type: LicenseeTypes;
}


export class NccLicensee extends BaseLicensee {
    public isFinancialAdministration: boolean = false;
    public garageAddress: string = null;
}

export class TaxiLicensee extends BaseLicensee {

    public shiftId: number;
    public subShiftId: number;
    public shiftName: string;
    public subShiftName: string;
}

export class LicenseeItem {
    public id: number;
    public vehiclePlate: string;
    public driverDisplayName: string;
    public taxiDriverAssociationName: string;
    public shiftName: string;
    public subShiftName: string;
    public releaseDate: string;
    public carFuelType: CarFuelTypes;
    public ownerAllDocuments: boolean;
    public collaboratorAllDocuments: boolean;
    public number: string;
    public status: LicenseeStateTypes;
    public type: LicenseeTypes;
    public isExpiring: boolean;
}

export abstract class LicenseeWrite {
    public licenseesIssuingOfficeId: LicenseesIssuingOffice;
    public releaseDate: string;
    public activityExpiredOnCause: string;
    public taxiDriverAssociationId: number;
    public note: string;
    public acronym: string;
    public isFamilyCollaboration: boolean = false;
    public number: string;
    public status: LicenseeStateTypes;
    public type: LicenseeTypes;
}

export class LicenseeTaxiWrite extends LicenseeWrite {
    public shiftId: number;
    public subShiftId: number;
}

export class LicenseeNCCWrite extends LicenseeWrite {
    public garageAddress: string;
    public isFinancialAdministration: boolean = false;
}

export class LicenseeTaxiVariationWrite extends LicenseeTaxiWrite {
    public variationNote: string;
}

export class LicenseeNCCVariationWrite extends LicenseeNCCWrite {
    public variationNote: string;
}

export enum LicenseeStateTypes {
    Created = "Created",
    Released = "Released",
    Suspended = "Suspended",
    Terminated = "Terminated"
}

export enum LicenseeTypes {
    Taxi = "Taxi",
    NCC_Auto = "NCC_Auto",
}


export class LicenseeSearchCriteria extends SearchCriteria {
    public id: number = null;
    public ids: number = null;
    public number: string;
    public vehiclePlate: string;
    public status: LicenseeStateTypes;
    public type: LicenseeTypes;
    public endFrom: string;
    public endTo: string;
    public releaseDateFrom: string;
    public releaseDateTo: string;
    public shiftId: number;
    public subShiftId: number;
    public taxiDriverAssociationId: number;
    public carFuelType: CarFuelTypes;
    public taxiDriverLastName: string;
    public isFamilyCollaboration: boolean;
    public isFinancialAdministration: boolean
    public garageAddress: string = null;
    public isSubstitution: boolean;
    public acronym: string;
    public hasActiveSubstitution: boolean;
}
//#endregion

export class LicenseesIssuingOfficeWrite {
    public description: string;
}

export class LicenseesIssuingOffice extends LicenseesIssuingOfficeWrite {
    public id: number;
}

export class IssuingOfficeSearchCriteria extends SearchCriteria {
    public description: string;
}

//#region Owner
export class TaxiDriverWrite {
    public personId: number;
    public shiftStarts: string;
    public collaborationType: FamilyCollaborationTypes;
    public documents: TaxiDriverDocument[] = [];
}

export class TaxiDriverVariation extends TaxiDriverWrite {
    public note: string = null;
}

export class TaxiDriver extends TaxiDriverWrite {
    public id: number;
    public personDisplayName: string;
    public extendedPersonDisplayName: string;
    public startDate: string;
    public note: string;
    public firstName: string;
    public lastName: string;
    public fiscalCode: string;
    public birthDate: string;
    public birthCity: string;
    public birthProvince: string;
    public phoneNumber: string;
    public emailOrPEC: string;
    public address: string;
    public zipCode: string;
    public residentCity: string;
    public residentProvince: string;
    public allDocuments: boolean;
}

export class TaxiDriverDocument {
    public id: number;
    public number: string;
    public type: DocumentTypes;
    public releasedBy: string;
    public validityDate: string;
}

export enum DocumentTypes {
    DrivingLicense = "DrivingLicense",
    KB = "KB",
    RoleRegistration = "RoleRegistration",
    CompanyArtisanRegister = "CompanyArtisanRegister"
}

export enum FamilyCollaborationTypes {
    A = 'A',
    B = 'B',
    F = 'F'
}
//#endregion

//#region Vehicles
export class VehicleWrite {
    public model: string;
    public licensePlate: string;
    public carFuelType: CarFuelTypes;
    public registrationDate: string;
    public inUseSince: string;
}
export class Vehicle extends VehicleWrite {
    public id: number;
    public licenseeId: number;
    public startDate: string;
    public note: string;
}

export class VehicleVariation extends Vehicle {
    public note: string;
}

export enum CarFuelTypes {
    Gasoline = "Gasoline",
    Diesel = "Diesel",
    Electric = "Electric",
    Hydrogen = "Hydrogen",
    BioFuels = "BioFuels",
    LiquefiedPetroleumGas = "LiquefiedPetroleumGas",
    CompressedNaturalGas = "CompressedNaturalGas",
    Hybrid = "Hybrid",
}
//#endregion

//#region Substitutions
export class DriverSubstitutionInfo {
    public driverId: number;
    public personDisplayName: string;
}

export abstract class BaseDriverSubstitution {
    public startDate: string;
    public endDate: string;
    public note: string;
}

export class DriverSubstitution extends BaseDriverSubstitution {
    public id: number;
    public substituteDriver: DriverSubstitutionInfo;
    public substituteDriverId: number;
    public status: SubstitutionsStatus;
    public isExpiring: boolean;
}

export class DriverSubstitutionWrite extends BaseDriverSubstitution {
    public substituteDriverId: number;
    public status: SubstitutionsStatus;
}

export enum SubstitutionsStatus {
    Active = "Active",
    ToActivate = "ToActivate",
    Terminated = "Terminated",
    Archived = "Archived"
}
//#endregion


export class Variation {
    public id: number;
    public releaseDate: string;
    public status: LicenseeStateTypes;
    public acronym: string;
    public shiftName: string;
    public subShiftName: string;
    public taxiDriverAssociationName: string;
    public isFamilyCollaboration: boolean;
    public licenseeNote: string;
    public activityExpiredOnCause: string;
    public note: string;
    public date: string;
    public garageAddress: string;
    public isFinancialAdministration: boolean;
}
//#region Financial Administration
export class FinancialAdministrationWrite {
    public drivers: PersonAutocomplete[];
    public legalPersonId: number;
    public legalPersonDisplayName: string;
}
export class FinancialAdministration extends FinancialAdministrationWrite {
    public id: number;
}
//#endregion 
//#region Templates

export class TemplateSmall {
    public id: number;
    public licenseeId: number;
    public templateId: number;
    public templateDescription: string;
    public lastUpdate: string;
    public protocolNumber: string;
    public protocolDate: string;
    public isSigned: boolean;
    public templateFileName: string;
    public executiveDigitalSignStatus: ExecutiveDigitalSignStatus;
}

export class ProtocolWrite {
    public subject: string;
    public recipient: string;
}

export class LeadProtocolDataInput {
    public year: number;
    public number: number;
    public externalDocumentId: string;
    public leadDocumentName: string;
}

export class LeadProtocolResult {
    public externalDocumentId: string;
    public leadDocumentName: string;
    public year: number;
    public number: number;
}

export class ProtocolMail {
    public sender: string;
    public subject: string;
    public text: string;
    public recipients: string[] = [];
}

export class DigitalSignCredential {
    public username: string;
    public password: string;
}

export class DigitalSign extends DigitalSignCredential {
    public otp: string;
}

export class CredentialUser {
    public firstName: string;
    public lastName: string;
}

export class MultipleSign {
    public requestRegisterIds: number[] = [];
    public credential: DigitalSign;
}

export class ProtocolInfo {
    public yearProtocol: number;
    public numberProtocol: number;
    public dateProtocolUTC: string;
}

export class ProcessCode {
    public id: number;
    public description: string;
    public code: string;
    public fullTextSearch: string;
}

export enum SignType {
    PADES = "PADES",
    CADES = "CADES"
}

export enum ExecutiveDigitalSignStatus {
    Required = "Required",
    NotRequired = "NotRequired",
    Signed = "Signed"
}

export enum EmailStatus {
    Draft = 1,
    ToSend,
    NotSent,
    WaitingToTakeCharge,
    TakenOver,
    NonAcceptance,
    NotDelivered,
    NotDeliveredForwarding,
    NoticeNotDelivered,
    PartiallyDelivered,
    Delivered
}

export class RequestRegisterSend {
    public ids: number[] = [];
    public executiveEmail: string;
}

export class TemplateExecutiveBase {
    public id: number;
    public templateDescription: string;
    public licenseeNumber: number;
    public licenseeType: LicenseeTypes;
}

export class TemplateExecutive extends TemplateExecutiveBase {
    public templateFileName: string;
    public lastUpdate: string;
}

export class ExternalDocument {
    public id: string;
    public title: string;
    public notes: string;
    public actionCode: string;
    public cupCode: string;
    public cigCode: string;
}

export class ExternalDocumentSearchCriteria extends SearchCriteria {
    public status: string;
    public protocolYear: string;
    public type: FolderType;
    public title: string;
    public cUPCode: string;
    public protocolNumber: number;
}

export enum FolderType {
    FASCICOLO_DETERMINE = "FASCICOLO_DETERMINE",
    FASCICOLO_GENERICO = "FASCICOLO_GENERICO"
}

export class ExternalDocumentAttachment {
    public name: string = null;
    public subject: string = null;
    public extension: string = null;
    public protocolNumber: string = null;
    public protocolDate: string = null;
    public protocolType: ProxyProtocolDirections = null;
    public isSigned: boolean;
    public signType: SignType;
}

export enum ProxyProtocolDirections {
    Incoming = "Incoming",
    Outgoing = "Outgoing",
    Internal = "Internal"
}
//#endregion

export class LicenseeEditData {
    constructor(public isTaxi: boolean, public isVariation: boolean = false, public licenseeId?: number) {

    }
}

export class LicenseeEditOwner {
    constructor(public licenseeId: number, public isEdit: boolean) {

    }
}

export class LicenseeEditFamilyCollaborator {
    constructor(public licenseeId: number, public isEdit: boolean) {

    }
}

export class LicenseeEditVehicle {
    constructor(public licenseeId: number, public isEdit: boolean) {

    }
}

export class LicenseeSubstitution {
    constructor(public licenseeId: number, public id?: number) {

    }
}


