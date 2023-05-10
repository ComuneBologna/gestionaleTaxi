import { Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { environment } from 'environments/environment';
import { LicenseeTemplateWrite, Tag, Template, TemplateBase, TemplateSearchCriteria, TemplateWrite } from '../models/templates.models';
import { HttpService, SearchResult } from "@asf/ng14-library";


@Injectable()
export class TemplatesService {
    constructor(private _httpService: HttpService) {
    }

    public searchTemplates = (criteria: TemplateSearchCriteria): Observable<SearchResult<Template>> => {
        return this._httpService.get(`${environment.apiUrl}/templates`, criteria);
    }

    public getAllTemplates = (): Observable<TemplateBase[]> => {
        return this._httpService.get(`${environment.apiUrl}/templates/all`);
    }

    public uploadTemplate = (file: File): Observable<string> => {
        return this._httpService.uploadFile(`${environment.apiUrl}/templates/upload`, file);
    }
    public save = (templateWrite: TemplateWrite, id: number): Observable<number> => {
        if (id) {
            this._httpService.put(`${environment.apiUrl}/templates/${id}`, templateWrite).pipe(map(m => id));
        }
        else {
            return this._httpService.post(`${environment.apiUrl}/templates`, templateWrite).pipe(map(m => m.id));
        }
    }

    public delete = (templateId: number): Observable<void> => {
        return this._httpService.delete(`${environment.apiUrl}/templates/${templateId}`);
    }

    public saveAndDownloadTemplate = (templateId: number, licenseeId: number, template: LicenseeTemplateWrite): Observable<string> => {
        return this._httpService.post(`${environment.apiUrl}/templates/${templateId}/licensees/${licenseeId}`, template);
    }

    public previewTemplate = (templateId: number, licenseeId: number, template: LicenseeTemplateWrite): Observable<string> => {
        return this._httpService.post(`${environment.apiUrl}/templates/${templateId}/licensees/${licenseeId}/draft`, template);
    }

    public uploadFile = (file: File, templateId: number, licenseeId: number): Observable<number> => {
        return this._httpService.uploadFile(`${environment.apiUrl}/templates/${templateId}/licensees/${licenseeId}/upload`, file);
    }

    public getTemplatePreview = (templateId: number, licenseeId: number): Observable<string> => {
        return this._httpService.get(`${environment.apiUrl}/templates/${templateId}/licensees/${licenseeId}/preview`);
    }

    public downloadTemplate = (templateId: number): Observable<string> => {
        return this._httpService.get(`${environment.apiUrl}/templates/${templateId}/download`);
    }

    public getAllTags = (): Observable<Tag[]> => {
        return this._httpService.get(`${environment.apiUrl}/templates/tags`);
    }

}