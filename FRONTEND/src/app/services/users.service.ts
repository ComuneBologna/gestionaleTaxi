import { Injectable } from '@angular/core';
import { environment } from 'environments/environment';
import { Observable } from 'rxjs';
import { HttpService, SearchResult } from '@asf/ng14-library';
import { map } from 'rxjs/operators';
import { User, UserSearchCriteria, UserWrite } from 'app/models/users.model';
import { TaxiDriver } from 'app/models/licensees.model';

@Injectable()
export class UsersService {
    constructor(private _httpService: HttpService) {
    }

    public searchUsers = (criteria: UserSearchCriteria): Observable<SearchResult<User>> => {
        return this._httpService.get(`${environment.apiUrl}/backofficeusers`, criteria);
    }

    public getAll = (): Observable<User[]> => {
        return this._httpService.get(`${environment.apiUrl}/backofficeusers`).pipe(map(result => result.items));
    }

    public getById = (id: string): Observable<User> => {
        return this._httpService.get(`${environment.apiUrl}/backofficeusers/${id}`);
    }
    public save = (user: UserWrite, id: string): Observable<User> => {
        if (id) {
            return this._httpService.put(`${environment.apiUrl}/backofficeusers/${id}`, user);
        }
        return this._httpService.post(`${environment.apiUrl}/backofficeusers`, user);
    }

    public delete = (id: string): Observable<any> => {
        return this._httpService.delete(`${environment.apiUrl}/backofficeusers/${id}`);
    }

    public getAllDrivers = (): Observable<TaxiDriver[]> => {
        return this._httpService.get(`${environment.apiUrl}/backofficeusers/drivers?itemsPerPage=9999&number=1`).pipe(map(result => result.items));
    }
}
