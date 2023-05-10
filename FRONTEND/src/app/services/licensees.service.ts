import { Injectable } from "@angular/core";
import { HttpService, SearchResult } from "@asf/ng14-library";
import { TemplateSmall, LicenseeSearchCriteria, LicenseeWrite, Vehicle, VehicleVariation, DriverSubstitution, Variation, LicenseesIssuingOffice, TaxiDriver, IssuingOfficeSearchCriteria, LicenseesIssuingOfficeWrite, LicenseeItem, LicenseeTypes, LicenseeTaxiVariationWrite, LicenseeNCCVariationWrite, TaxiLicensee, NccLicensee, TaxiDriverVariation, TaxiDriverWrite, VehicleWrite, DriverSubstitutionWrite, FinancialAdministration, FinancialAdministrationWrite, ProtocolWrite, DigitalSignCredential, ProtocolMail, ProtocolInfo, TemplateExecutive, RequestRegisterSend, TemplateExecutiveBase, ExternalDocumentSearchCriteria, ExternalDocument, ExternalDocumentAttachment, LeadProtocolDataInput, LeadProtocolResult, ProcessCode, MultipleSign, EmailStatus, ExecutiveDigitalSignStatus, CredentialUser } from "app/models/licensees.model";
import { PersonAutocomplete } from "app/models/people.model";
import { ShiftSmall, SubShiftSmall } from "app/models/shifts.model";
import { ManageTemplatesSearchCriteria, TemplateSearchCriteria } from "app/models/templates.models";
import { environment } from "environments/environment";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";


@Injectable()
export class LicenseesService {
    constructor(private _httpService: HttpService) {

    }

    //#region Licensee
    public searchLicensees = (criteria: LicenseeSearchCriteria): Observable<SearchResult<LicenseeItem>> => {
        return this._httpService.get(`${environment.apiUrl}/licensees`, criteria);
    }

    public getLicenseebyId = (id: number, isTaxi: boolean): Observable<TaxiLicensee | NccLicensee> => {
        return this._httpService.get(`${environment.apiUrl}/licensees${isTaxi ? '' : '/ncc'}/${id}`);
    }

    public saveLicensee = (licensee: LicenseeWrite, id: number = null): Observable<number> => {
        if (id) {
            return this._httpService.put(`${environment.apiUrl}/licensees${licensee.type == LicenseeTypes.NCC_Auto ? '/ncc' : ''}/${id}`, licensee).pipe(map(m => id));
        }
        else {
            return this._httpService.post(`${environment.apiUrl}/licensees${licensee.type == LicenseeTypes.NCC_Auto ? '/ncc' : ''}`, licensee).pipe(map(m => m));
        }
    }

    public saveVariation = (variation: LicenseeTaxiVariationWrite | LicenseeNCCVariationWrite, licenseeId: number = null): Observable<number> => {
        return this._httpService.put(`${environment.apiUrl}/licensees${variation.type == LicenseeTypes.NCC_Auto ? '/ncc' : ''}/${licenseeId}/variation`, variation).pipe(map(m => licenseeId));
    }

    public licenseeNccVariation = (variation: LicenseeWrite, licenseeId: number = null): Observable<number> => {
        return this._httpService.put(`${environment.apiUrl}/licensees/${licenseeId}/variation`, variation);
    }

    public getlicenseeVariations = (licenseeId: number): Observable<Variation[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/variations`).pipe(map(result => result.items));
    }

    public getlicenseeNccVariations = (licenseeId: number): Observable<Variation[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/variations`).pipe(map(result => result.items));
    }

    public areAllDocumentPresent = (licenseeId: number, driverId: number): Observable<boolean> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/documents/${driverId}/exists`);
    }

    public licenceeRenew = (licenseeId: number): Observable<any> => {
        return this._httpService.put(`${environment.apiUrl}/licensees/${licenseeId}/renew`, null);
    }
    //#endregion

    //#region Owners
    public getOwner = (licenseeId: number): Observable<TaxiDriver> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/owner`);
    }

    public getOwnersVariations = (licenseeId: number): Observable<TaxiDriver[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/owner/variations`);
    }

    public saveOwner = (owner: TaxiDriverWrite, edit: boolean, licenseeId: number): Observable<any> => {
        if (edit) {
            return this._httpService.put(`${environment.apiUrl}/licensees/${licenseeId}/owner`, owner);
        }
        else {
            return this._httpService.post(`${environment.apiUrl}/licensees/${licenseeId}/owner`, owner);
        }
    }

    public editOwnerVariation = (owner: TaxiDriverVariation, licenseeId: number = null): Observable<number> => {
        return this._httpService.put(`${environment.apiUrl}/licensees/${licenseeId}/owner/variations`, owner);
    }
    //#endregion

    //#region Vehicles
    public getVehicle = (licenseeId: number): Observable<Vehicle> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/vehicle`);
    }

    public getVehiclesVariations = (licenseeId: number): Observable<Vehicle[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/vehicle/variations`);
    }

    public saveVehicle = (vehicle: VehicleWrite, licenseeId: number, edit: boolean): Observable<any> => {
        if (edit) {
            return this._httpService.put(`${environment.apiUrl}/licensees/${licenseeId}/vehicle`, vehicle);
        }
        else {
            return this._httpService.post(`${environment.apiUrl}/licensees/${licenseeId}/vehicle`, vehicle);
        }
    }

    public saveVehicleVariation = (vehicle: VehicleVariation, licenseeId: number = null): Observable<any> => {
        return this._httpService.put(`${environment.apiUrl}/licensees/${licenseeId}/vehicle/variations`, vehicle);
    }
    //#endregion

    //#region Shifts
    public getAllShifts = (): Observable<ShiftSmall[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/shifts`);
    }

    public getSubShiftsByShiftId = (shiftId: number): Observable<SubShiftSmall[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/shifts/${shiftId}/subshifts`);
    }
    //#endregion

    //#region Family collborators
    public getFamilyCollaborator = (licenseeId: number): Observable<TaxiDriver> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/familycollaborators`);
    }

    public getFamilyCollaboratorVariations = (licenseeId: number): Observable<TaxiDriver[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/familycollaborators/variations`);
    }

    public saveFamilyCollaborator = (driver: TaxiDriverWrite, licenseeId: number, edit: boolean): Observable<any> => {
        if (edit) {
            return this._httpService.put(`${environment.apiUrl}/licensees/${licenseeId}/familycollaborators`, driver);
        }
        else {
            return this._httpService.post(`${environment.apiUrl}/licensees/${licenseeId}/familycollaborators`, driver);
        }
    }

    public editFamilyCollaboratorVariation = (driver: TaxiDriverWrite, licenseeId: number): Observable<number> => {
        return this._httpService.put(`${environment.apiUrl}/licensees/${licenseeId}/familycollaborators/variation`, driver);
    }
    //#endregion

    //#region Substitution
    public getSubstitutionsDrivers = (licenseeId: number): Observable<DriverSubstitution[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/substitutions`).pipe(map(result => result.items));
    }

    public getSubstitutionsDriverById = (licenseeId: number, substitutionId: number): Observable<DriverSubstitution> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/substitutions/${substitutionId}`);
    }

    public saveDriverSubstitution = (driver: DriverSubstitutionWrite, licenseeId: number, id?: number): Observable<number> => {
        if (id) {
            return this._httpService.put(`${environment.apiUrl}/licensees/${licenseeId}/substitutions/${id}`, driver).pipe(map(m => id))
        }
        else {
            return this._httpService.post(`${environment.apiUrl}/licensees/${licenseeId}/substitutions`, driver).pipe(map(m => m.id));
        }
    }

    public deleteDriverSubstitution = (licenseeId: number, sustitutionId: number): Observable<void> => {
        return this._httpService.delete(`${environment.apiUrl}/licensees/${licenseeId}/substitutions/${sustitutionId}`);
    }
    //#endregion

    //#region  Licensees Issuing Offices
    public getIssuingOffices = (): Observable<LicenseesIssuingOffice[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/licenseesissuingoffices`);
    }

    public searchIssuingOffices = (criteria: IssuingOfficeSearchCriteria): Observable<SearchResult<LicenseesIssuingOffice>> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/licenseesissuingoffices/search`, criteria);
    }

    public getIssuingOfficeById = (id: number): Observable<LicenseesIssuingOffice> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/licenseesissuingoffices/${id}`);
    }

    public saveIssuingOffice = (issuingOffice: LicenseesIssuingOfficeWrite, id: number = null): Observable<number> => {
        if (id) {
            return this._httpService.put(`${environment.apiUrl}/licensees/licenseesissuingoffices/${id}`, issuingOffice);
        }
        else {
            return this._httpService.post(`${environment.apiUrl}/licensees/licenseesissuingoffices`, issuingOffice);
        }
    }

    public deleteIssuingOffice = (id: number): Observable<void> => {
        return this._httpService.delete(`${environment.apiUrl}/licensees/licenseesissuingoffices/${id}`);
    }
    //#endregion

    //#region Financial Administrations
    public getFinancialAdministration = (licenseeId: number): Observable<FinancialAdministration> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/financialadministrations`);
    }

    public saveFinancialAdministration = (economicManagement: FinancialAdministrationWrite, licenseeId: number, edit: boolean): Observable<any> => {
        if (edit) {
            return this._httpService.put(`${environment.apiUrl}/licensees/${licenseeId}/financialadministrations`, economicManagement);
        }
        else {
            return this._httpService.post(`${environment.apiUrl}/licensees/${licenseeId}/financialadministrations`, economicManagement);
        }
    }
    //#endregion

    //#region Common
    public taxiDriversAutocomplete = (licenseeId: number, text: string): Observable<PersonAutocomplete[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/autocomplete`, { fullTextSearch: text });
    }
    public getTaxiDriverByLicenseeId = (licenseeId: number): Observable<PersonAutocomplete[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/drivers`);
    }
    //#endregion

    //#region Search
    public exportSearch = (criteria: LicenseeSearchCriteria): Observable<any> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/export`, criteria);
    }
    //#endregion

    //#region Templates
    public getTemplatesByLicenseeId = (licenseeId: number): Observable<TemplateSmall[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/requestsregisters`);
    }

    public deleteTemplate = (licenseeId: number, requestRegisterId: number): Observable<string[]> => {
        return this._httpService.delete(`${environment.apiUrl}/licensees/${licenseeId}/requestsregisters/${requestRegisterId}`);
    }

    public downloadTemplatesByLicenseeId = (requestRegisterId: number): Observable<Blob> => {
        return this._httpService.get(`${environment.apiUrl}/templates/requestsregisters/${requestRegisterId}/download`, null, null, 'blob');
    }

    public protocol = (requestRegisterId: number, data: ProtocolWrite): Observable<ProtocolInfo> => {
        return this._httpService.post(`${environment.apiUrl}/protocol/requestsregisters/${requestRegisterId}/protocols`, data);
    }

    public sendProtocol = (requestRegisterId: number, data: ProtocolMail): Observable<string> => {
        return this._httpService.post(`${environment.apiUrl}/protocol/requestsregisters/${requestRegisterId}/protocols/send`, data);
    }

    public checkEmailStatus = (requestRegisterId: number): Observable<EmailStatus> => {
        return this._httpService.get(`${environment.apiUrl}/protocol/requestsregisters/${requestRegisterId}/checkemailstatus`, null);
    }

    public recipientsAutocomplete = (licenseeId: number, mail: string): Observable<string[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/recipients/autocomplete?mail=${mail}`)
    }

    public recipientsList = (licenseeId: number): Observable<string[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/recipients`)
    }

    public startProcess = (requestRegisterId: number, processTypeCode: number): Observable<string> => {
        return this._httpService.post(`${environment.apiUrl}/protocol/requestsregisters/${requestRegisterId}/protocols/startprocess/${processTypeCode}`, null);
    }

    public stopProcess = (requestRegisterId: number, processTypeCode: number): Observable<string> => {
        return this._httpService.post(`${environment.apiUrl}/protocol/requestsregisters/${requestRegisterId}/protocols/stopprocess/${processTypeCode}`, null);
    }

    public getProcessCode = (fullTextSearch: string): Observable<ProcessCode[]> => {
        return this._httpService.get(`${environment.apiUrl}/protocol/processcodes/autocomplete?fullTextSearch=${fullTextSearch}`);
    }

    public realiseInChargeOfTheFolder = (requestRegisterId: number): Observable<string> => {
        return this._httpService.post(`${environment.apiUrl}/protocol/requestsregisters/${requestRegisterId}/protocols/releaseinchargeofthefolder`, null);
    }

    public sign = (data: DigitalSignCredential): Observable<string> => {
        return this._httpService.post(`${environment.apiUrl}/digitalsign/otp`, data);
    }

    public multipleSignWithOtp = (singType: string, data: MultipleSign): Observable<any> => {
        return this._httpService.post(`${environment.apiUrl}/digitalsign/requestsregisters/sign/${singType}`, data);
    }

    public verifySignError = (requestRegisterId: number): Observable<string> => {
        return this._httpService.get(`${environment.apiUrl}/digitalsign/requestsregisters/${requestRegisterId}/signerrors`);
    }

    public searchCredentials = (criteria: any): Observable<SearchResult<CredentialUser>> => {
        return this._httpService.get(`${environment.apiUrl}/digitalsign/credentials`, criteria);
    };

    public getCredential = (): Observable<DigitalSignCredential> => {
        return this._httpService.get(`${environment.apiUrl}/digitalsign/credential`);
    };

    public saveCredential = (credential: DigitalSignCredential): Observable<number> => {
        return this._httpService.put(`${environment.apiUrl}/digitalsign/credentials`, credential);

    };

    public deleteCredential = (): Observable<any> => {
        return this._httpService.delete(`${environment.apiUrl}/digitalsign/credentials`);
    };

    public searchDocumentsToSign = (criteria: TemplateSearchCriteria): Observable<SearchResult<TemplateExecutive>> => {
        return this._httpService.get(`${environment.apiUrl}/templates/requestsregisters/tosign`, criteria);
    }

    public searchManageTemplates = (criteria: ManageTemplatesSearchCriteria): Observable<SearchResult<TemplateSmall>> => {
        return this._httpService.get(`${environment.apiUrl}/templates/requestsregisters`, criteria);
    };

    public removeDocumentToSing = (requestRegisterId: number): Observable<string> => {
        return this._httpService.put(`${environment.apiUrl}/templates/requestsregisters/${requestRegisterId}/rollback`, null);
    };

    public getDocumentsExecutive = (licenseeId: number): Observable<TemplateExecutiveBase[]> => {
        return this._httpService.get(`${environment.apiUrl}/licensees/${licenseeId}/requestsregisters/autocomplete`);
    }

    public sendNotificationToExecutive = (licenseeId: number, data: RequestRegisterSend): Observable<string> => {
        return this._httpService.post(`${environment.apiUrl}/licensees/${licenseeId}/requestsregisters/send`, data);
    }

    public documentToSign = (licenseeId: number, requestRegisterId: number): Observable<string> => {
        return this._httpService.put(`${environment.apiUrl}/licensees/${licenseeId}/requestsregisters/${requestRegisterId}/tosign`, null);
    }

    public searchLeadProtocol = (criteria: ExternalDocumentSearchCriteria): Observable<SearchResult<ExternalDocument>> => {
        return this._httpService.get(`${environment.apiUrl}/protocol/external/documents/search`, criteria);
    }

    public connectToLeadDocument = (requestRegisterId: number, data: LeadProtocolDataInput): Observable<LeadProtocolResult> => {
        return this._httpService.post(`${environment.apiUrl}/protocol/requestsregisters/${requestRegisterId}/leaddocuments/group`, data);
    }

    public getConnectedLeadProtocol = (requestRegisterId: string): Observable<LeadProtocolResult> => {
        return this._httpService.get(`${environment.apiUrl}/protocol/requestsregisters/${requestRegisterId}/leaddocuments`);
    }

    public getDocumentByIdConsole = (externaldocumentId: string): Observable<ExternalDocumentAttachment[]> => {
        return this._httpService.get(`${environment.apiUrl}/protocol/external/documents/${externaldocumentId}/attachments`);
    }
    //#endregion

}