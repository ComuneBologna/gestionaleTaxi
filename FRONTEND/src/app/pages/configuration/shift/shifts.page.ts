import { Component, ViewEncapsulation } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { DataTableAction, DataTableColumn, RemoteDataTableManager, DataTableUtils, DialogService, BaseTablePageComponent, IDataTableManager, IconConstants } from '@asf/ng14-library';
import { fuseAnimations } from '@asf/ng14-library';
import { ShiftItem, ShiftsSearchCriteria } from 'app/models/shifts.model';
import { ShiftsService } from 'app/services/shifts.service';
import { ShiftComponent } from './shift.component';


@Component({
    selector: 'shifts-page',
    templateUrl: './shifts.page.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class ShiftsPage extends BaseTablePageComponent<ShiftItem, Filters> {

    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];
    public form: UntypedFormGroup;
    public canManage: boolean = false;

    constructor(private _dialogService: DialogService, private _shiftsService: ShiftsService, activatedRoute: ActivatedRoute) {
        super(activatedRoute)
        this.filters = new Filters();
    }

    public initialize = (): void => {
        this.canManage = true;
        this.prepareTable();
    }

    protected getDataTableManager = (searchCriteria: ShiftsSearchCriteria): IDataTableManager<ShiftItem> => {
        return new RemoteDataTableManager(this._shiftsService.getShift, this.setSearchCriteria, searchCriteria);
    }

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createStringColumn("name", "shift.name", true));
        columns.push(DataTableUtils.createIntColumn("durationInHour", "shift.durationInHour", true));
        this.tableColumns.push(...columns);
        this.tableActions.push(DataTableUtils.createAction("common.edit", IconConstants.dataTable.edit, this.edit));
        this.tableActions.push(DataTableUtils.createAction("common.delete", IconConstants.dataTable.delete, this.delete));
    }

    public add() {
        this.showDialog();
    }

    public edit = (row: ShiftItem): void => {
        this.showDialog(row.id);
    }

    public showDialog = (id?: number): void => {
        this._dialogService.show(ShiftComponent, {
            panelClass: "modal-lg",
            data: id,
            callback: result => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            }
        });
    }

    public delete = (row: ShiftItem): void => {
        this._dialogService.showConfirm("shift.deleteShiftTitle", "shift.deleteShiftMessage", {
            callback: (result) => {
                if (result) {
                    this._shiftsService.delete(row.id).subscribe(result => {
                        this.dataTableManager.startReload();
                    });
                }
            }
        },
            [row.name]);
    }

    public search = (): void => {
        this.dataTableManager.startSearch();
    }

    public reset = (): void => {
        this.filters.name = null;
        this.filters.durationInHour = null;
        this.dataTableManager.startSearch();
    }

}
class Filters {
    public name: string = null;
    public durationInHour: number = null;
}