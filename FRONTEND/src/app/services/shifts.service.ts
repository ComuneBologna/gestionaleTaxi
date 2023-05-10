import { Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { environment } from 'environments/environment';
import { HttpService, SearchResult } from "@asf/ng14-library";
import { Shift, ShiftItem, ShiftsSearchCriteria, ShiftToWrite } from "app/models/shifts.model";


@Injectable()
export class ShiftsService {
    constructor(private _httpService: HttpService) {
    }

    public getShift = (shiftsSearchCriteria: ShiftsSearchCriteria): Observable<SearchResult<ShiftItem>> => {
        return this._httpService.get(`${environment.apiUrl}/shifts`, shiftsSearchCriteria);
    }

    public getShiftById = (id: number): Observable<Shift> => {
        return this._httpService.get(`${environment.apiUrl}/shifts/${id}`);
    }

    public save = (shifts: ShiftToWrite, id: number = null): Observable<number> => {
        if (id) {
            return this._httpService.put(`${environment.apiUrl}/shifts/${id}`, shifts).pipe(map(m => id));
        }
        else {
            return this._httpService.post(`${environment.apiUrl}/shifts`, shifts).pipe(map(m => m));
        }
    }

    public delete = (id: number): Observable<void> => {
        return this._httpService.delete(`${environment.apiUrl}/shifts/${id}`);
    }

}