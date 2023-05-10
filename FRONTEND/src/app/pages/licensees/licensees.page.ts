import { Component, ViewEncapsulation } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DataTableAction, DataTableColumn, fuseAnimations, RemoteDataTableManager, DataTableUtils, DialogService, BaseTablePageComponent, IDataTableManager, SelectListitem, EnumUtils, SpinnerService, FilesUtils, SearchResult, SnackBarService, RightBarService } from '@asf/ng14-library';
import { forkJoin, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CarFuelTypes as CarFuelTypes, LicenseeEditData, LicenseeItem, LicenseeSearchCriteria, LicenseeStateTypes, LicenseeTypes, SubstitutionsStatus } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';
import { AssociationsService } from 'app/services/associations.service';
import { EditLicenseeComponent } from './edit-licensee.component';
import { DetailComponent } from './details/detail.component';


@Component({
    selector: 'licensees-page',
    templateUrl: './licensees.page.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class LicenseesPage extends BaseTablePageComponent<LicenseeItem, Filters> {

    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];
    public form: UntypedFormGroup;
    public containerType: SelectListitem[] = [];
    public stateItems: SelectListitem[] = [];
    public typeItems: SelectListitem[] = [];
    public subTypeItems: SelectListitem[] = [];
    public associationItems: SelectListitem[] = [];
    public carFuelTypeItems: SelectListitem[] = [];
    public type: LicenseeTypes = LicenseeTypes.Taxi;
    public url: string;
    public collaborationItems: SelectListitem[] = [];
    public economicManagementItems: SelectListitem[] = [];
    public substitutionItems: SelectListitem[] = [];
    public hasSubstitutionItems: SelectListitem[] = [];

    public get isTaxi(): boolean {
        return this.type == LicenseeTypes.Taxi;
    }

    constructor(private _dialogService: DialogService, private _associationsService: AssociationsService, private _licenseesService: LicenseesService, private _router: Router, private _translateService: TranslateService,
        private _spinnerService: SpinnerService, private _snackBarService: SnackBarService, activatedRoute: ActivatedRoute, private _rightBarService: RightBarService) {
        super(activatedRoute, false);
        this.filters = new Filters();
    }

    public initialize = (): void => {
        this.hasSubstitutionItems = [{id: true, label: 'Si'}, {id: false, label: 'No'}];
        this.stateItems = EnumUtils.toSelectListitems(LicenseeStateTypes, "LicenseeStateTypes", this._translateService);
        this.carFuelTypeItems = EnumUtils.toSelectListitems(CarFuelTypes, "CarFuelTypes", this._translateService);
        this.collaborationItems = [{id: true, label: 'Si'}, {id: false, label: 'No'}];
        this.economicManagementItems = [{id: true, label: 'Si'}, {id: false, label: 'No'}]
        this.substitutionItems = [{id: true, label: 'Si'}, {id: false, label: 'No'}];
        forkJoin([this._associationsService.getAllAssociations(), this._licenseesService.getAllShifts()]).subscribe(results => {
            this.associationItems = [...results[0].map(m => new SelectListitem(m.id, m.name))];
            this.typeItems = [...results[1].map(m => new SelectListitem(m.id, m.name))];
        });
        this.on(this.activatedRoute.params.subscribe(params => {
            // this.filters = new Filters();
            let oldType = localStorage.getItem('type');
            this.type = params["type"] == "ncc" ? LicenseeTypes.NCC_Auto : LicenseeTypes.Taxi;
            if(oldType != params['type']){
                this.filters = new Filters();
            }
            this.prepareTable();
            this.dataTableManager.startSearch();
        }));
    }

    protected getDataTableManager = (searchCriteria: LicenseeSearchCriteria): IDataTableManager<LicenseeItem> => {
        return new RemoteDataTableManager(this.loadLicensees, this.setSearchCriteria, searchCriteria);
    }

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createStringColumn("number", "licensee.licenseeNumber", true));
        columns.push(DataTableUtils.createStringColumn("driverDisplayName", "common.nominative", true));
        columns.push(DataTableUtils.createDateColumn("releaseDate", "licensee.releaseDate", true))
        if (this.type == LicenseeTypes.Taxi) {
            columns.push(DataTableUtils.createStringColumn("shiftName", "shift.shift", true));
            columns.push(DataTableUtils.createStringColumn("subShiftName", "shift.subshift", true));
        }
        columns.push(DataTableUtils.createStringColumn("taxiDriverAssociationName", "licensee.categoryAssociationName", true));
        columns.push(DataTableUtils.createStringColumn("vehiclePlate", "licensee.vehicleLicensePlate", true));
        columns.push(DataTableUtils.createEnumColumn("carFuelType", "vehicle.carFuelType", "CarFuelTypes", true));
        columns.push(DataTableUtils.createIconNoPropertyTableColumn("licensee.documentCompleted", (row) => { return row.ownerAllDocuments ? 'far:square' : 'far:square-check' }));
        this.tableColumns = [...columns];
        this.tableActions = [
            DataTableUtils.createAction("common.compile", 'fas:pen-to-square', this.edit),
            DataTableUtils.createAction("licensee.renew", 'fas:repeat', this.renew, (row) => { return row.isExpiring }, false)
        ];
    }

    public loadShift = (): Observable<any> => {
        return this._licenseesService.getAllShifts().pipe(tap(result => {
            for (let i = 0; i < result.length; i++) {
                this.typeItems.push(new SelectListitem(result[i].id, result[i].name));
            }
        }));
    }

    public rowClick = (row: LicenseeItem): void => {
        this._rightBarService.show(DetailComponent, {
            data: row,
            hasBackdrop: true,
            closeOnBackdrop: true,
            
            callback: (result) => {
            },
        });
    }

    public getSubShifts = (shiftId: number): void => {
        this.subTypeItems = [];
        this._licenseesService.getSubShiftsByShiftId(shiftId).subscribe(result => {
            this.subTypeItems = [...result.map(m => new SelectListitem(m.id, m.name))];
        });
    }

    public add() {
        this._dialogService.show(EditLicenseeComponent, {
            panelClass: "modal-lg",
            data: new LicenseeEditData(this.isTaxi),
            callback: result => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            }
        });
    }

    public renew = (row: LicenseeItem): void => {
        this._dialogService.showConfirm("licensee.renewLicenseeTitle", "licensee.renewLicenseeMessage", {
            callback: (result) => {
                if (result) {
                    this._licenseesService.licenceeRenew(row.id).subscribe(result => {
                        this._snackBarService.success("common.successfull");
                        this.dataTableManager.startReload();
                    });
                }
            }
        },
            [row.number]
        );
    }

    public loadLicensees = (criteria: LicenseeSearchCriteria): Observable<SearchResult<LicenseeItem>> => {
        criteria.type = this.type;
        return this._licenseesService.searchLicensees(criteria);
    }

    public edit = (row: LicenseeItem): void => {
        this._router.navigateWithReturnUrl(["/", "licensees", this.isTaxi ? "taxi" : "ncc", row.id], this.getReturnUrl());
    }

    public search = (): void => {
        this.dataTableManager.startSearch();
    }

    public reset = (): void => {
        this.filters.number = null;
        this.filters.vehiclePlate = null;
        this.filters.status = null;
        this.filters.type = this.type;
        this.filters.endFrom = null;
        this.filters.endTo = null;
        this.filters.releaseDateFrom = null;
        this.filters.releaseDateTo = null;
        this.filters.shiftId = null;
        this.filters.subShiftId = null;
        this.filters.taxiDriverAssociationId = null;
        this.filters.carFuelType = null;
        this.filters.isFamilyCollaboration = null;
        this.filters.isFinancialAdministration = null;
        this.filters.garageAddress = null;
        this.filters.isSubstitution = null;
        this.filters.taxiDriverLastName = null;
        this.filters.acronym = null;
        this.filters.hasActiveSubstitution = null;
        this.dataTableManager.startSearch();
    }

    public export = (): void => {
        let criteria = new LicenseeSearchCriteria();
        criteria.type = this.type;
        criteria.number = this.filters.number
        criteria.vehiclePlate = this.filters.vehiclePlate;
        criteria.status = this.filters.status;
        criteria.endFrom = this.filters.endFrom;
        criteria.endTo = this.filters.endTo;
        criteria.releaseDateFrom = this.filters.releaseDateFrom;
        criteria.releaseDateTo = this.filters.releaseDateTo;
        criteria.shiftId = this.filters.shiftId;
        criteria.subShiftId = this.filters.subShiftId;
        criteria.taxiDriverAssociationId = this.filters.taxiDriverAssociationId;
        criteria.carFuelType = this.filters.carFuelType;
        criteria.isFamilyCollaboration = this.filters.isFamilyCollaboration;
        criteria.isFinancialAdministration = this.filters.isFinancialAdministration;
        criteria.garageAddress = this.filters.garageAddress;
        criteria.isSubstitution = this.filters.isSubstitution;
        criteria.taxiDriverLastName = this.filters.taxiDriverLastName;
        criteria.acronym = this.filters.acronym;
        criteria.hasActiveSubstitution = this.filters.hasActiveSubstitution;
        this._spinnerService.show();
        this._licenseesService.exportSearch(criteria).subscribe(result => {
            FilesUtils.createLink(result, "_blank");
            this._spinnerService.hide();
        })
    }
}
class Filters {
    public number: string = null;
    public vehiclePlate: string = null;
    public status: LicenseeStateTypes = null;
    public type: LicenseeTypes = null;
    public endFrom: string = null;
    public endTo: string = null;
    public releaseDateFrom: string = null;
    public releaseDateTo: string = null;
    public shiftId: number = null;
    public subShiftId: number = null;
    public taxiDriverAssociationId: number = null;
    public carFuelType: CarFuelTypes = null;
    public taxiDriverLastName: string = null;
    public isFamilyCollaboration: boolean;
    public isFinancialAdministration: boolean;
    public garageAddress: string = null;
    public isSubstitution: boolean;
    public acronym: string = null;
    public hasActiveSubstitution: boolean;
}