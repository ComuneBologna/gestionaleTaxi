import { Component, ViewEncapsulation } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DataTableAction, DataTableColumn, RemoteDataTableManager, DataTableUtils, DialogService, BaseTablePageComponent, IDataTableManager, SelectListitem, IconConstants } from '@asf/ng14-library';
import { LicenseeWrite } from 'app/models/licensees.model';
import { AssociationComponent } from './association.component';
import { CategoryAssociation, CategoryAssociationSearchCriteria } from 'app/models/category-associations.model';
import { AssociationsService } from 'app/services/associations.service';
import { fuseAnimations } from '@asf/ng14-library';


@Component({
    selector: 'associations-page',
    templateUrl: './associations.page.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class AssociationsPage extends BaseTablePageComponent<CategoryAssociation, Filters> {

    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];
    public form: UntypedFormGroup;
    public canManage: boolean = false;
    public containerType: SelectListitem[] = [];

    constructor(private _dialogService: DialogService, private _associationService: AssociationsService, private translateService: TranslateService, activatedRoute: ActivatedRoute) {
        super(activatedRoute)
        this.filters = new Filters();
    }

    public initialize = (): void => {
        this.canManage = true;
        this.prepareTable();
    }

    protected getDataTableManager = (searchCriteria: CategoryAssociationSearchCriteria): IDataTableManager<CategoryAssociation> => {
        return new RemoteDataTableManager(this._associationService.searchAssociations, this.setSearchCriteria, searchCriteria);
    }

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createStringColumn("name", "person.firstName", true));
        columns.push(DataTableUtils.createStringColumn("fiscalCode", "person.fiscalCode", true));
        columns.push(DataTableUtils.createStringColumn("email", "person.email", true));
        columns.push(DataTableUtils.createStringColumn("telephoneNumber", "person.phoneNumber", true));
        this.tableColumns.push(...columns);
        this.tableActions.push(DataTableUtils.createAction("common.edit", IconConstants.dataTable.edit, this.edit));
        this.tableActions.push(DataTableUtils.createAction("common.delete", IconConstants.dataTable.delete, this.delete));
    }

    public add() {
        this.showDialog();
    }

    public edit = (row: CategoryAssociation): void => {
        this.showDialog(row.id);
    }

    public showDialog = (id?: number): void => {
        this._dialogService.show(AssociationComponent, {
            panelClass: "modal-sm",
            data: id,
            callback: result => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            }
        });
    }

    public delete = (row: CategoryAssociation): void => {
        this._dialogService.showConfirm("association.deleteAssociationTitle", "association.deleteAssociatioMessage", {
            callback: (result) => {
                if (result) {
                    this._associationService.deleteAssociationsById(row.id).subscribe(result => {
                        this.dataTableManager.startReload();
                    });
                }
            }
        },
            [row.name]
        );
    }

    public search = (): void => {
        this.dataTableManager.startSearch();
    }

    public clearFilters = (): void => {
        this.filters.fiscalCode = null;
        this.filters.name = null;
        this.dataTableManager.startSearch();
    }


}
class Filters {
    public name: string = null;
    public fiscalCode: string = null;
}