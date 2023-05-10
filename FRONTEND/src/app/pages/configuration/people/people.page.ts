import { Component, ViewEncapsulation } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { DataTableAction, DataTableColumn, RemoteDataTableManager, DataTableUtils, DialogService, BaseTablePageComponent, IDataTableManager, SelectListitem, SpinnerService, FilesUtils, IconConstants, EnumUtils } from '@asf/ng14-library';
import { fuseAnimations } from '@asf/ng14-library';
import { TranslateService } from '@ngx-translate/core';
import { PeopleSearchCriteria, PersonItem, PersonTypes } from 'app/models/people.model';
import { PeopleService } from 'app/services/people.service';
import { PersonComponent } from './person.component';

@Component({
    selector: 'people-page',
    templateUrl: './people.page.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class PeoplePage extends BaseTablePageComponent<PersonItem, Filters> {

    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];
    public form: UntypedFormGroup;
    public canManage: boolean = false;
    public containerType: SelectListitem[] = [];
    public typeItems: SelectListitem[] = [];

    constructor(private _dialogService: DialogService, private _peopleService: PeopleService, activatedRoute: ActivatedRoute, private _spinnerService: SpinnerService,
        private _translateServices: TranslateService) {
        super(activatedRoute)
        this.filters = new Filters();
    }

    public initialize = (): void => {
        this.canManage = true;
        this.typeItems = EnumUtils.toSelectListitems(PersonTypes, "PersonTypes", this._translateServices)
        this.prepareTable();
    }

    protected getDataTableManager = (searchCriteria: PeopleSearchCriteria): IDataTableManager<PersonItem> => {
        return new RemoteDataTableManager(this._peopleService.searchPeople, this.setSearchCriteria, searchCriteria);
    }

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createStringColumn("displayName", "common.nominative", true));
        columns.push(DataTableUtils.createStringColumn("fiscalCode", "person.fiscalCode", true));
        columns.push(DataTableUtils.createStringColumn("emailOrPEC", "person.pecOrEmail", true));
        columns.push(DataTableUtils.createStringColumn("address", "person.address", true));
        columns.push(DataTableUtils.createStringColumn("zipCode", "person.postalCode", true));
        columns.push(DataTableUtils.createStringColumn("residentCity", "person.city", true));
        this.tableColumns.push(...columns);
        this.tableActions.push(DataTableUtils.createAction("common.edit", IconConstants.dataTable.edit, this.edit));

    }

    public add() {
        this.showDialog();
    }

    public edit = (row: PersonItem): void => {
        this.showDialog(row.id);
    }


    public showDialog = (id?: number): void => {
        this._dialogService.show(PersonComponent, {
            panelClass: "modal-lg",
            data: {id: id, islegal: null},
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
        this.filters.personDescription = null;
        this.filters.phoneNumber = null;
        this.filters.emailOrPEC = null;
        this.filters.fiscalCode = null;
        this.filters.personType = null;
        this.dataTableManager.startSearch();
    }

    public export = (): void => {
        let criteria = new PeopleSearchCriteria();
        criteria.personDescription = this.filters.personDescription
        criteria.phoneNumber = this.filters.phoneNumber;
        criteria.emailOrPec = this.filters.emailOrPEC;
        criteria.fiscalCode = this.filters.fiscalCode;
        criteria.personType = this.filters.personType;
        this._spinnerService.show();
        this._peopleService.exportSearch(criteria).subscribe(result => {
            FilesUtils.createLink(result, "_blank");
            this._spinnerService.hide();
        })
    }
}

class Filters {
    public personDescription: string = null;
    public phoneNumber: string = null;
    public emailOrPEC: string = null;
    public fiscalCode: string = null;
    public personType: PersonTypes = null;
}