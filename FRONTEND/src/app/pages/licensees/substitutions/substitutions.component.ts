import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { BaseComponent, SpinnerService, DialogService, ListTableManager, DataTableColumn, DataTableAction, DataTableUtils, IconConstants, SnackBarService } from '@asf/ng14-library';
import { Observable } from 'rxjs';
import { fuseAnimations } from '@asf/ng14-library';
import { DriverSubstitution, LicenseeSubstitution, NccLicensee, SubstitutionsStatus, TaxiLicensee } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';
import { SubstitutionComponent } from './substitution.component';

@Component({
    selector: 'substitutions',
    templateUrl: './substitutions.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class SubstitutionsComponent extends BaseComponent implements OnInit {
    public tableManager: ListTableManager;
    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];

    private _licensee: TaxiLicensee | NccLicensee;
    @Input()
    public get licensee(): TaxiLicensee | NccLicensee {
        return this._licensee;
    }
    public set licensee(value: TaxiLicensee | NccLicensee) {
        this._licensee = value;
        this.tableManager.startSearch();
    }

    constructor(private _dialogService: DialogService, private _licenseesService: LicenseesService, private _spinnerService: SpinnerService, private _snackBarService: SnackBarService) {
        super();
        this.prepareTable();
    }
    ngOnInit(): void {
    }
    private loadSubstitutions = (): Observable<DriverSubstitution[]> => {
        return this._licenseesService.getSubstitutionsDrivers(this.licensee.id)
    }

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createDateColumn("startDate", "drivers.substitutionStartDate", false));
        columns.push(DataTableUtils.createDateColumn("endDate", "drivers.substitutionEndDate", false));
        columns.push(DataTableUtils.createStringColumn("substituteDriver.personDisplayName", "drivers.driverName", false));
        columns.push(DataTableUtils.createEnumColumn("status", "common.status", "SubstitutionsStatus", false));
        columns.push(DataTableUtils.createStringColumn("note", "common.note", false));
        columns.push(DataTableUtils.createIconNoPropertyTableColumn("common.isExpiring", this.isExpiring))
        this.tableColumns.push(...columns);
        this.tableActions.push(DataTableUtils.createAction("common.edit", IconConstants.dataTable.edit, this.editSubstitution));
        this.tableActions.push(DataTableUtils.createAction("common.delete", IconConstants.dataTable.delete, this.deleteSubstitution));
        this.tableActions.push(DataTableUtils.createAction("common.archive", 'fas:box-archive', this.archive, (row) => row.status == SubstitutionsStatus.Terminated, false));

        this.tableManager = new ListTableManager(this.loadSubstitutions);
    }

    public isExpiring = (row: DriverSubstitution): string => {
        return row.isExpiring ? 'fas:triangle-exclamation' : ''
    }

    public archive = (row: DriverSubstitution): void => {
        this._spinnerService.show();
        const data = row;
        data.status = SubstitutionsStatus.Archived
        this._licenseesService.saveDriverSubstitution(data, this.licensee.id, row.id).subscribe(() => {
            this._snackBarService.success("common.successfull");
            this.tableManager.startReload();
            this._spinnerService.hide();
        });
    }

    public addSubstitution = (): void => {
        this.showSubstitutionDialog();
    }
    private editSubstitution = (row: DriverSubstitution): void => {
        this.showSubstitutionDialog(row.id);
    }

    private showSubstitutionDialog = (id?: number): void => {
        this._dialogService.show(SubstitutionComponent, {
            panelClass: "modal-xl",
            data: new LicenseeSubstitution(this.licensee.id, id),
            callback: result => {
                if (result) {
                    this.tableManager.startReload();
                }
            }
        });
    }

    public deleteSubstitution = (row: DriverSubstitution): void => {
        this._dialogService.showConfirm("drivers.deleteDriverSubstitutionTitle", "drivers.deleteDriverSubstitutionMessage", {
            callback: (result) => {
                if (result) {
                    this._licenseesService.deleteDriverSubstitution(this.licensee.id, row.id).subscribe(result => {
                        this.tableManager.startReload();
                    });
                }
            }
        },
            [row.substituteDriver.personDisplayName]);
    }
}