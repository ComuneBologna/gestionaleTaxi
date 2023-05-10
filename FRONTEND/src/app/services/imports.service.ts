import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Attachment, Import, ImportsSearchCriteria, ImportWrite } from "../models/imports.model";
import { environment } from 'environments/environment';
import { HttpService, SearchResult } from "@asf/ng14-library";


@Injectable()
export class ImportService {
    constructor(private _httpService: HttpService) {
    }

    public getImports = (criteria: ImportsSearchCriteria): Observable<SearchResult<Import>> => {
        return this._httpService.get(`${environment.apiUrl}/imports`, criteria);
    }

    public getErrors = (importId: number): Observable<any> => {
        return this._httpService.get(`${environment.apiUrl}/imports/${importId}/errors`);
    }

    public getExcelErrors = (importId: number): Observable<any> => {
        return this._httpService.get(`${environment.apiUrl}/imports/${importId}/excelerrors`);
    }

    public uploadFile = (file: File): Observable<Attachment> => {
        return this._httpService.uploadFile(`${environment.apiUrl}/imports/upload`, file);
    }

    public save = (measures: ImportWrite, id: number = null): Observable<number> => {
        return this._httpService.post(`${environment.apiUrl}/imports`, measures);
    }

}