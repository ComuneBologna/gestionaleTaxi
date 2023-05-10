import { Component, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DataTableAction, DataTableColumn, RemoteDataTableManager, DataTableUtils, DialogService, SelectListitem, BaseTablePageComponent, IDataTableManager, EnumUtils } from '@asf/ng14-library';
import { ImportComponent } from './import.component';
import { Import, ImportsSearchCriteria, ImportStatus } from 'app/models/imports.model';
import { ImportService } from 'app/services/imports.service';
import { fuseAnimations } from '@asf/ng14-library';

@Component({
    selector: 'imports',
    templateUrl: './imports.page.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class ImportsPage extends BaseTablePageComponent<Import, Filters> {

    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];
    public canManage: boolean = false;
    public statusItems: SelectListitem[] = [];

    constructor(private _dialogService: DialogService, private _router: Router, private _importService: ImportService, activatedRoute: ActivatedRoute, private _translateService: TranslateService) {
        super(activatedRoute)
        this.filters = new Filters();
    }

    protected initialize = (): void => {
        this.canManage = true;
        this.prepareTable();
        this.statusItems = [...EnumUtils.toSelectListitems(ImportStatus, "ImportStatus", this._translateService)];
    }

    protected getDataTableManager = (searchCriteria: ImportsSearchCriteria): IDataTableManager<Import> => {
        return new RemoteDataTableManager(this._importService.getImports, this.setSearchCriteria, searchCriteria);
    }

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createStringColumn("description", "common.description", true));
        columns.push(DataTableUtils.createEnumColumn("status", "imports.importStatus", "ImportStatus", true));
        columns.push(DataTableUtils.createDateTimeColumn("createdAt", "imports.createdAt", true));
        this.tableColumns.push(...columns);
        if (this.canManage) {
            this.tableActions.push(DataTableUtils.createAction("imports.downloadLog", "cloud_download", this.detail, (row) => { return row.status == ImportStatus.ImportedWithErrors; }, false));
        }
    }

    public detail = (row: Import): void => {
        this._importService.getExcelErrors(row.id).subscribe(result => this.createLink(result));
    }

    private createLink = (url: string): void => {
        var a = document.createElement('a');
        a.href = url;
        a.download = url;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    }

    public import = (): void => {
        this._dialogService.show(ImportComponent, {
            panelClass: "modal-md",
            data: null,
            callback: result => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            }
        });
    }

    public search = (): void => {
        this.dataTableManager.startSearch();
    }

    public reset = (): void => {
        this.filters.description = null;
        this.filters.status = null;
        this.filters.createdAtFrom = null;
        this.filters.createdAtTo = null;
        this.dataTableManager.startSearch();
    }
}
class Filters {
    public description: string;
    public status: ImportStatus;
    public createdAtFrom: string;
    public createdAtTo: string;
}