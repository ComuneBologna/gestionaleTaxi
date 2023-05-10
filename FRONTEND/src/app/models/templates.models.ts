import { SearchCriteria } from "@asf/ng14-library";
import { Attachment } from "./imports.model";
import { ExecutiveDigitalSignStatus, LicenseeTypes } from "./licensees.model";

export class TemplateBase {
    public id: number;
    public description: string;
}
export class Template extends TemplateBase {
    public fileName: string;
}

export class TemplateWrite {
    public attachment: Attachment;
    public description: string;
}

export class LicenseeTemplateWrite {
    public id: number;
    public year: string;
    public date: string;
    public months: string;
    public dateFrom: string;
    public dateTo: string;
    public protocolNumber: string;
    public protocolDate: string;
    public internalProtocolNumber: string;
    public internalProtocolDate: string;
    public internalProtocol: string;
    public note: string;
    public collaboratorRelationship: string;
    public freeText: string;
    public day: string;
}

export class Tag {
    public description: string;
    public value: string;
}


export class Protocol {
    constructor(public internalProtocol: string) {

    }
}

export class TemplateSearchCriteria extends SearchCriteria {
    public description: string;
    public fileName: string;
    public executiveDigitalSignStatus: ExecutiveDigitalSignStatus;
}

export class ManageTemplatesSearchCriteria extends SearchCriteria {
    public licenseeId: number;
    public templateId: number;
    public description: string;
    public lastUpdateFrom: string;
    public lastUpdateTo: string;
    public executiveDigitalSignStatus: ExecutiveDigitalSignStatus;
    public licenseeType: LicenseeTypes;
    public licenseeNumber: number;
}