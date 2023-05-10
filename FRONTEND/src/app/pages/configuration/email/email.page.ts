import { Component, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BaseTablePageComponent, DataTableAction, DataTableColumn, DataTableUtils, DialogService, IDataTableManager, RemoteDataTableManager } from '@asf/ng14-library';
import { TranslateService } from '@ngx-translate/core';
import { ProtocolEmail, ProtocolEmailSearchCriteria } from 'app/models/email.model';
import { EmailService } from 'app/services/email.service';
import { EditEmailComponent } from './edit-email.component';

@Component({
    selector: 'email',
    templateUrl: './email.page.html',
    encapsulation: ViewEncapsulation.None,
})
export class EmailPage extends BaseTablePageComponent<ProtocolEmail, Filters> {
    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];

    constructor(
        activatedRoute: ActivatedRoute,
        private _emailService: EmailService,
        private translateService: TranslateService,
        private _dialogService: DialogService
    ) {
        super(activatedRoute);
        this.filters = new Filters();
    }

    public initialize = (): void => {
        this.prepareTable();
    };

    protected getDataTableManager = (searchCriteria: ProtocolEmailSearchCriteria): IDataTableManager<ProtocolEmail> => {
        return new RemoteDataTableManager(this._emailService.searchEmail, this.setSearchCriteria, searchCriteria);
    };

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createStringColumn('email', 'users.email', true));
        columns.push(DataTableUtils.createStringColumn('description', 'common.description', true));
        columns.push(DataTableUtils.createBooleanColumn('active', 'users.enabled', true));
        this.tableColumns.push(...columns);
        this.tableActions.push(
            DataTableUtils.createAction('common.edit', 'edit', this.edit)
        );
        this.tableActions.push(
            DataTableUtils.createAction('common.delete', 'delete', this.delete)
        );
    };

    public add = (): void => {
        this._dialogService.show(EditEmailComponent, {
            data: null,
            callback: (result) => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            },
        });
    };

    public edit = (row: ProtocolEmail): void => {
        this._dialogService.show(EditEmailComponent, {
            data: row.id,
            callback: (result) => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            },
        });
    };

    public delete = (row: ProtocolEmail): void => {
        let title = this.translateService.instant(
            'email.deleteEmailTitle'
        );
        let message = (<string>(
            this.translateService.instant('email.deleteEmailMessage')
        )).format([row.email]);
        this._dialogService.showConfirm(title, message, {
            callback: (result) => {
                if (result) {
                    this._emailService
                        .deleteEmail(row.id)
                        .subscribe((result) => {
                            this.dataTableManager.startReload();
                        });
                }
            },
        });
    };

    public search = (): void => {
        this.dataTableManager.startSearch();
    };

    public clearFilters = (): void => {
        this.filters.email = null;
        this.filters.description = null;
        this.dataTableManager.startSearch();
    };
}
class Filters {
    public description: string = null;
    public email: string = null;
}
