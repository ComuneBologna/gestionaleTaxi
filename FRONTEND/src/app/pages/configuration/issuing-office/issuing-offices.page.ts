import { Component, ViewEncapsulation } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { DataTableAction, DataTableColumn, RemoteDataTableManager, DataTableUtils, DialogService, BaseTablePageComponent, IDataTableManager, SelectListitem, IconConstants } from '@asf/ng14-library';
import { fuseAnimations } from '@asf/ng14-library';
import { IssuingOfficeSearchCriteria as IssuingOfficesSearchCriteria, LicenseesIssuingOffice } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';
import { IssuingOfficeComponent } from './issuing-office.component';


@Component({
    selector: 'issuing-offices-page',
    templateUrl: './issuing-offices.page.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class IssuingOfficesPage extends BaseTablePageComponent<LicenseesIssuingOffice, Filters> {

    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];
    public form: UntypedFormGroup;
    public canManage: boolean = false;
    public containerType: SelectListitem[] = [];

    constructor(private _dialogService: DialogService, private _licenseesService: LicenseesService, activatedRoute: ActivatedRoute) {
        super(activatedRoute)
        this.filters = new Filters();
    }

    public initialize = (): void => {
        this.canManage = true;
        this.prepareTable();
    }

    protected getDataTableManager = (searchCriteria: IssuingOfficesSearchCriteria): IDataTableManager<LicenseesIssuingOffice> => {
        return new RemoteDataTableManager(this._licenseesService.searchIssuingOffices, this.setSearchCriteria, searchCriteria);
    }

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createStringColumn("description", "common.description", true));
        this.tableColumns.push(...columns);
        this.tableActions.push(DataTableUtils.createAction("common.edit", IconConstants.dataTable.edit, this.edit));
        this.tableActions.push(DataTableUtils.createAction("common.delete", IconConstants.dataTable.delete, this.delete));
    }

    public add() {
        this.showDialog();
    }

    public edit = (row: LicenseesIssuingOffice): void => {
        this.showDialog(row.id);
    }

    public showDialog = (id?: number): void => {
        this._dialogService.show(IssuingOfficeComponent, {
            panelClass: "modal-md",
            data: id,
            callback: result => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            }
        });
    }

    public delete = (row: LicenseesIssuingOffice): void => {
        this._dialogService.showConfirm("issuingOffice.deleteIssuingOfficeTitle", "issuingOffice.deleteIssuingOfficeMessage", {
            callback: (result) => {
                if (result) {
                    this._licenseesService.deleteIssuingOffice(row.id).subscribe(result => {
                        this.dataTableManager.startReload();
                    });
                }
            }
        }, [row.description]);
    }

    public search = (): void => {
        this.dataTableManager.startSearch();
    }

    public reset = (): void => {
        this.filters.description = null;
        this.dataTableManager.startSearch();
    }

}
class Filters {
    public description: string = null;
}