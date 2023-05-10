import { Injectable } from '@angular/core';
import { HttpService, SearchResult } from '@asf/ng14-library';
import { ProtocolEmail, ProtocolEmailSearchCriteria } from 'app/models/email.model';
import {  Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable()
export class EmailService {
    constructor(private _httpService: HttpService) {
    }

    public searchEmail = (criteria: ProtocolEmailSearchCriteria): Observable<SearchResult<ProtocolEmail>> => {
        return this._httpService.get(`${environment.apiUrl}/protocol/emails`, criteria)
    }

    public getEmail = (): Observable<ProtocolEmail[]> => {
        return this._httpService.get(`${environment.apiUrl}/protocol/emails/list`)
    }

    public getEmailById = (id: number): Observable<ProtocolEmail> => {
        return this._httpService.get(`${environment.apiUrl}/protocol/emails/${id}`)
    }

    public saveEmail = (email: ProtocolEmail, id: number): Observable<number> => {
        if (id) {
            return this._httpService.put(`${environment.apiUrl}/protocol/emails/${id}`, email)
        }
        else {
            return this._httpService.post(`${environment.apiUrl}/protocol/emails`, email)
        }
    }

    public deleteEmail = (id: number): Observable<any> => {
        return this._httpService.delete(`${environment.apiUrl}/protocol/emails/${id}`);
    }

}
