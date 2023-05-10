import { SearchCriteria } from "@asf/ng14-library";

export class Import {
    public id: number;
    public status: ImportStatus;
    public path: string;
    public createdAt: string;
    public description: string;
}

export class ImportWrite {
    public description: string;
    public attachmentId: string;
}

export class Attachment {
    map(arg0: (m: any) => any): any {
        throw new Error('Method not implemented.');
    }
    public id: number;
    public mimeType: string;
    public externalPath: string;
    public fileName: string;
}

export class ImportsSearchCriteria extends SearchCriteria {
    public id: number;
    public ids: number[] = [];
    public description: string;
    public status: ImportStatus;
    public createdAtFrom: string;
    public createdAtTo: string;
}

export enum ImportStatus {
    Uploaded = "Uploaded",
    InProgress = "InProgress",
    Imported = "Imported",
    ImportedWithErrors = "ImportedWithErrors"
}
