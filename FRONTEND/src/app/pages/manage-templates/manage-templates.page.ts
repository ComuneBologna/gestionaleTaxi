import { Component, ViewEncapsulation } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import {
    DataTableAction, DataTableColumn, RemoteDataTableManager, DataTableUtils, DialogService, BaseTablePageComponent, IDataTableManager,
    SnackBarService, ProfileService, IconConstants, SpinnerService, SelectListitem, EnumUtils
} from '@asf/ng14-library';
import { fuseAnimations } from '@asf/ng14-library';
import { ManageTemplatesSearchCriteria } from 'app/models/templates.models';
import { SignDocumentComponent } from '../licensees/templates/sign-document.component';
import { LicenseesService } from 'app/services/licensees.service';
import { ExecutiveDigitalSignStatus, LicenseeTypes, TemplateSmall } from 'app/models/licensees.model';
import { PermissionCodes } from 'app/models/common.model';
import { EmailstatusComponent } from '../licensees/templates/email-status.component';
import { ProtocolComponent } from '../licensees/templates/protocol.component';
import { ProcessComponent } from '../licensees/templates/process.component';
import { ConnectedProtocolComponent } from '../licensees/templates/connected-protocol.component';
import { SearchLeadProtocolComponent } from '../licensees/templates/search-lead-protocol.component';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'manage-templates-page',
    templateUrl: './manage-templates.page.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None,
})
export class ManageTemplatesPage extends BaseTablePageComponent<TemplateSmall, Filters> {
    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];
    public form: UntypedFormGroup;
    public canManage: boolean;
    public showNotification: boolean = false;
    public executiveStatusItems: SelectListitem[] = [];
    public typeItems: SelectListitem[] = [];

    constructor(private _dialogService: DialogService, private _licenseesService: LicenseesService, private _profileService: ProfileService,
        private _snackBarService: SnackBarService, activatedRoute: ActivatedRoute, private _spinnerService: SpinnerService, private _translateService: TranslateService) {
        super(activatedRoute);
        this.filters = new Filters();
    }

    public initialize = (): void => {
        this.executiveStatusItems = EnumUtils.toSelectListitems(ExecutiveDigitalSignStatus, 'ExecutiveDigitalSignStatus', this._translateService);
        this.typeItems = EnumUtils.toSelectListitems(LicenseeTypes, 'LicenseeTypes', this._translateService);
        this._profileService.getPermissions().subscribe((result) => {
            this.canManage = result.findIndex(f => f == PermissionCodes.Taxi_Executive) > 0;
        });
        this.prepareTable();
    };

    protected getDataTableManager = (searchCriteria: ManageTemplatesSearchCriteria): IDataTableManager<TemplateSmall> => {
        return new RemoteDataTableManager(this._licenseesService.searchManageTemplates, this.setSearchCriteria, searchCriteria);
    };

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createEnumColumn('licenseeType', 'licensee.licenseeType', 'LicenseeTypes', true));
        columns.push(DataTableUtils.createStringColumn('licenseeNumber', 'licensee.licenseeNumber', true));
        columns.push(DataTableUtils.createStringColumn('templateDescription', 'common.description', true));
        columns.push(DataTableUtils.createDateColumn('lastUpdate', 'requests.lastUpdate', true));
        columns.push(DataTableUtils.createStringColumn('protocolNumber', 'requests.protocolNumber', true));
        columns.push(DataTableUtils.createDateColumn('protocolDate', 'requests.protocolDate', true));
        //columns.push(DataTableUtils.createBooleanColumn('isSigned', 'requests.isSigned', true));
        columns.push(DataTableUtils.createEnumColumn('executiveDigitalSignStatus', 'template.executiveSign', 'ExecutiveDigitalSignStatus', true));
        this.tableColumns.push(...columns);
        this.tableActions = [
            DataTableUtils.createAction('template.searchLeadProtocol', 'fas:folder-tree', this.leadProtocol),
            DataTableUtils.createAction('template.sign', 'fas:signature', this.sign, (row) => { return row.protocolNumber == null; }, false),
            DataTableUtils.createAction('template.executiveSign', 'fas:user-pen', this.executiveSign, (row) => { return row.protocolNumber == null; }, false),
            DataTableUtils.createAction('template.send', 'fas:paper-plane', this.send, (row) => { return row.protocolNumber != null; }, false),
            DataTableUtils.createAction('template.checkEmailStatus', 'fas:check-to-slot', this.emailStatus, (row) => { return row.protocolNumber != null; }, false),
            DataTableUtils.createAction('template.protocol', 'fas:stamp', this.protocol, (row) => { return row.protocolNumber == null; }, false),
            DataTableUtils.createAction('template.startProcess', 'fas:play', this.startProcess, (row) => { return row.protocolNumber != null; }, false),
            DataTableUtils.createAction('template.stopProcess', 'fas:stop', this.stopProcess, (row) => { return row.protocolNumber != null; }, false),
            DataTableUtils.createAction('template.releaseInChargeOfTheFolder', 'fas:retweet', this.releaseInChargeOfTheFolder, (row) => { return row.protocolNumber != null; }, false),
            DataTableUtils.createAction('common.delete', IconConstants.dataTable.delete, this.delete, (row) => { return row.protocolNumber == null; }, false),
            DataTableUtils.createAction('template.rollback', 'fas:rotate-left', this.remove, (row) => { return row.executiveDigitalSignStatus == ExecutiveDigitalSignStatus.Required }, false),
            DataTableUtils.createAction('common.download', "fas:download", this.download,),];
    };

    public remove = (row: TemplateSmall): void => {
        this._licenseesService.removeDocumentToSing(row.id).subscribe(result => {
            this._snackBarService.success('common.operationSuccessfull')
            this.dataTableManager.startReload();
        })
    }

    public executiveSign = (template: TemplateSmall): void => {
        this._licenseesService.documentToSign(template.licenseeId, template.id).subscribe((result) => {
            this.showNotification = true;
            this.dataTableManager.startReload();
        });
    };

    public startProcess = (template: TemplateSmall): void => {
        this._dialogService.show(ProcessComponent, {
            panelClass: 'modal-xl',
            data: { id: template.id, isStarting: true },
            callback: (result) => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            },
        });
    };

    public stopProcess = (template: TemplateSmall): void => {
        this._dialogService.show(ProcessComponent, {
            panelClass: 'modal-xl',
            data: { id: template.id, isStarting: false },
            callback: (result) => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            },
        });
    };

    public leadProtocol = (template: TemplateSmall): void => {
        this._licenseesService
            .getConnectedLeadProtocol(template.id.toString())
            .subscribe((result) => {
                this._spinnerService.show();
                if (result) {
                    this._dialogService.show(ConnectedProtocolComponent, {
                        data: { result: result, displayResult: false },
                    });
                } else if (template.protocolNumber == null) {
                    this._dialogService.show(SearchLeadProtocolComponent, {
                        panelClass: 'modal-lg',
                        data: template.id,
                        callback: (result) => {
                            if (result) {
                                this.dataTableManager.startReload();
                            }
                        },
                    });
                } else {
                    this._dialogService.show(ConnectedProtocolComponent, {
                        data: { result: result, displayResult: true },
                    });
                }
                this._spinnerService.hide();
            });
    };

    public releaseInChargeOfTheFolder = (template: TemplateSmall): void => {
        this._licenseesService
            .realiseInChargeOfTheFolder(template.id)
            .subscribe((result) => {
                this._snackBarService.success('common.successfull');
                this.dataTableManager.startReload();
            });
    };

    public download = (template: TemplateSmall): void => {
        this._licenseesService
            .downloadTemplatesByLicenseeId(template.id)
            .subscribe((result) => {
                const blob = new Blob([result], { type: result.type });
                const url = window.URL.createObjectURL(blob);
                var a = document.createElement('a');
                a.href = url;
                a.download = url;
                a.setAttribute('download', template.templateFileName);
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
            });
    };

    public protocol = (template: TemplateSmall): void => {
        this._dialogService.show(ProtocolComponent, {
            panelClass: 'modal-lg',
            data: { template: template, send: false },
            callback: (result) => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            },
        });
    };

    public sign = (template: TemplateSmall): void => {
        this._dialogService.show(SignDocumentComponent, {
            data: [template.id],
            callback: (result) => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            },
        });
    };

    public send = (template: TemplateSmall): void => {
        this._dialogService.show(ProtocolComponent, {
            data: { template: template, send: true, licenseeId: template.licenseeId },
            callback: (result) => {
                if (result) {
                }
            },
        });
    };

    public emailStatus = (template: TemplateSmall): void => {
        this._dialogService.show(EmailstatusComponent, {
            panelClass: 'modal-sm',
            data: template.id,
            callback: (result) => { },
        });
    };

    public delete = (row: TemplateSmall): void => {
        this._dialogService.showConfirm(
            'association.deleteAssociationTitle',
            'association.deleteAssociatioMessage',
            {
                callback: (result) => {
                    if (result) {
                        this._licenseesService
                            .deleteTemplate(row.licenseeId, row.id)
                            .subscribe((result) => {
                                this.dataTableManager.startReload();
                            });
                    }
                },
            },
            [row.templateDescription]
        );
    };

    public search = (): void => {
        this.dataTableManager.startSearch();
    };

    public clearFilters = (): void => {
        this.filters.licenseeId = null;
        this.filters.templateId = null;
        this.filters.description = null;
        this.filters.lastUpdateFrom = null;
        this.filters.lastUpdateTo = null;
        this.filters.executiveDigitalSignStatus = null;
        this.filters.licenseeType = null;
        this.filters.licenseeNumber = null;
        this.dataTableManager.startSearch();
    };
}
class Filters {
    public licenseeId: number;
    public templateId: number;
    public description: string;
    public lastUpdateFrom: string;
    public lastUpdateTo: string;
    public executiveDigitalSignStatus: ExecutiveDigitalSignStatus;
    public licenseeType: LicenseeTypes;
    public licenseeNumber: number;
}
