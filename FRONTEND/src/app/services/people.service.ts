import { Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { environment } from 'environments/environment';
import { Person, PersonAutocomplete, PersonItem, PeopleSearchCriteria, PersonWrite, PersonTypes } from "app/models/people.model";
import { HttpService, SearchResult } from "@asf/ng14-library";


@Injectable()
export class PeopleService {
    constructor(private _httpService: HttpService) { }

    public searchPeople = (criteria: PeopleSearchCriteria): Observable<SearchResult<PersonItem>> => {
        return this._httpService.get(`${environment.apiUrl}/people`, criteria);
    }

    public people = (textToSearch: string, type?: PersonTypes): Observable<PersonAutocomplete[]> => {
        return this._httpService.get(`${environment.apiUrl}/people/autocomplete`, {
            fullTextSearch: textToSearch,
            type: type
        });
    }

    public getPersonById = (id: number): Observable<Person> => {
        return this._httpService.get(`${environment.apiUrl}/people/${id}`);
    }

    public savePerson = (person: PersonWrite, id: number = null, type: PersonTypes): Observable<number> => {
        if (id) {
            return this._httpService.put(`${environment.apiUrl}/people/${id}/${type}`, person).pipe(map(m => id));
        }
        else {
            return this._httpService.post(`${environment.apiUrl}/people/${type}`, person).pipe(map(m => m));
        }
    }

    public exportSearch = (criteria: PeopleSearchCriteria): Observable<string> => {
        return this._httpService.get(`${environment.apiUrl}/people/export`, criteria);
    }


}