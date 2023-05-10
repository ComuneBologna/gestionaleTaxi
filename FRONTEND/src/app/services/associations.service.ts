import { Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { environment } from 'environments/environment';
import { HttpService, SearchResult } from "@asf/ng14-library";
import { CategoryAssociation, CategoryAssociationSearchCriteria, CategoryAssociationWrite } from "app/models/category-associations.model";


@Injectable()
export class AssociationsService {
    constructor(private _httpService: HttpService) {

    }

    public searchAssociations = (criteria: CategoryAssociationSearchCriteria): Observable<SearchResult<CategoryAssociation>> => {
        return this._httpService.get(`${environment.apiUrl}/taxidriverassociations`, criteria);
    }

    public getAllAssociations = (): Observable<CategoryAssociation[]> => {
        const criteria = new CategoryAssociationSearchCriteria();
        criteria.itemsPerPage = 999999;
        return this.searchAssociations(criteria).pipe(map(m => m.items));
    }

    public getAssociationsById = (id: number): Observable<CategoryAssociation> => {
        return this._httpService.get(`${environment.apiUrl}/taxidriverassociations/${id}`);
    }

    public save = (association: CategoryAssociationWrite, id: number = null): Observable<number> => {
        if (id) {
            return this._httpService.put(`${environment.apiUrl}/taxidriverassociations/${id}`, association);
        }
        else {
            return this._httpService.post(`${environment.apiUrl}/taxidriverassociations`, association);
        }
    }

    public deleteAssociationsById = (id: number): Observable<void> => {
        return this._httpService.delete(`${environment.apiUrl}/taxidriverassociations/${id}`);
    }
}