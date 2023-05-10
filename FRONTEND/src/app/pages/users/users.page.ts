import { Component, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { EditUserComponent } from './edit-user.component';
import { fuseAnimations, IconConstants } from '@asf/ng14-library';
import { BaseTablePageComponent, DataTableAction, DataTableColumn, DataTableUtils, DialogService, EnumUtils, IDataTableManager, ILocalDataTableManager, LocalDataTableManager, RemoteDataTableManager, SearchCriteria, SelectListitem, SpinnerService } from '@asf/ng14-library';
import { PermissionCodes } from 'app/models/common.model';
import { User, UserSearchCriteria } from 'app/models/users.model';
import { UsersService } from 'app/services/users.service';


@Component({
    selector: 'users',
    templateUrl: './users.page.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class UsersPage extends BaseTablePageComponent<User, Filters> {

    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];
    public canManage: boolean = false;
    public permissionItems: SelectListitem[] = [];

    constructor(private _dialogService: DialogService, private _spinnerService: SpinnerService, private _usersService: UsersService, private translateService: TranslateService, activatedRoute: ActivatedRoute) {
        super(activatedRoute)
        this.filters = new Filters();
    }

    public initialize = (): void => {
        this.canManage = true;
        this.prepareTable();
        this.permissionItems = [...EnumUtils.toSelectListitems(PermissionCodes, "PermissionCodes", this.translateService).filter(f => f.id != PermissionCodes.Tenant_Admin)];
    }

    protected getDataTableManager = (criteria: UserSearchCriteria): IDataTableManager<User> => {
        return new RemoteDataTableManager(this._usersService.searchUsers, this.setSearchCriteria, criteria);
    }

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createStringColumn("firstName", "users.firstName", true));
        columns.push(DataTableUtils.createStringColumn("lastName", "users.lastName", true));
        columns.push(DataTableUtils.createStringColumn("email", "users.email", true));
        columns.push(DataTableUtils.createEnumColumn("permissionCode", "users.role", "PermissionCodes", true));
        this.tableColumns.push(...columns);
        if (this.canManage) {
            this.tableActions.push(DataTableUtils.createAction("common.edit", IconConstants.dataTable.edit, this.edit));
            this.tableActions.push(DataTableUtils.createAction("common.delete", IconConstants.dataTable.delete, this.delete));
        }
    }

    public add = (): void => {
        this._dialogService.show(EditUserComponent, {
            data: null,
            callback: result => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            }
        });
    }

    public edit = (row: User): void => {
        this._dialogService.show(EditUserComponent, {
            data: row.id,
            callback: result => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            }
        });
    }

    public delete = (row: User): void => {
        let title = this.translateService.instant("users.deleteUserTitle");
        let message = (<string>this.translateService.instant("users.deleteUserMessage")).format([row.firstName + " " + row.lastName])
        this._dialogService.showConfirm(title, message, {
            callback: (result) => {
                if (result) {
                    this._spinnerService.show();
                    this._usersService.delete(row.id).subscribe(result => {
                        this.dataTableManager.startReload();
                        this._spinnerService.hide();
                    });
                }
            }
        });
    }

    public search = (): void => {
        this.dataTableManager.startSearch();
    }

    public clearFilters = (): void => {
        this.filters.fullTextSearch = null;
        this.filters.permissionCode = null;
        this.dataTableManager.startSearch();
    }

}
class Filters {
    public fullTextSearch: string;
    public permissionCode: string;
}