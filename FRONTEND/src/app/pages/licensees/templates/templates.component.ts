import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { BaseComponent, ListTableManager, DataTableColumn, DataTableAction, DataTableUtils, DialogService, SnackBarService, SpinnerService, IconConstants } from '@asf/ng14-library';
import { Observable } from 'rxjs';
import { fuseAnimations } from '@asf/ng14-library';
import { NccLicensee, TaxiLicensee, TemplateSmall } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';

import { ProtocolComponent } from './protocol.component';
import { SignDocumentComponent } from './sign-document.component';
import { SendNotificationComponent } from './send-notification.component';
import { ProcessComponent } from './process.component';
import { SearchLeadProtocolComponent } from './search-lead-protocol.component';
import { ConnectedProtocolComponent } from './connected-protocol.component';
import { EmailstatusComponent } from './email-status.component';

@Component({
    selector: 'templates',
    templateUrl: './templates.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None,
    styles: ["mat-row{cursor:pointer;}"]
})

export class TemplatesComponent extends BaseComponent implements OnInit {
    public tableManager: ListTableManager;
    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];
    public showNotification: boolean = false;

    private _licensee: TaxiLicensee | NccLicensee;
    @Input()
    public get licensee(): TaxiLicensee | NccLicensee {
        return this._licensee;
    }
    public set licensee(value: TaxiLicensee | NccLicensee) {
        this._licensee = value;
        this.tableManager.startSearch();
    }

    constructor(private _licenseesService: LicenseesService, private _dialogService: DialogService, private _snackBarService: SnackBarService, private _spinnerService: SpinnerService) {
        super();
        this.prepareTable();
    }
    ngOnInit(): void {
        this._licenseesService.getDocumentsExecutive(this.licensee.id).subscribe(result => {
            this.showNotification = result.any();
        })
    }
    private loadTemplates = (): Observable<TemplateSmall[]> => {
        return this._licenseesService.getTemplatesByLicenseeId(this.licensee.id)
    }

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createStringColumn("templateDescription", "common.description", true));
        columns.push(DataTableUtils.createDateColumn("lastUpdate", "requests.lastUpdate", true));
        columns.push(DataTableUtils.createStringColumn("protocolNumber", "requests.protocolNumber", true));
        columns.push(DataTableUtils.createDateColumn("protocolDate", "requests.protocolDate", true));
        columns.push(DataTableUtils.createBooleanColumn("isSigned", "requests.isSigned", true));
        columns.push(DataTableUtils.createEnumColumn("executiveDigitalSignStatus", "template.executiveSign", "ExecutiveDigitalSignStatus", true));
        this.tableColumns.push(...columns);
        this.tableActions = [
            DataTableUtils.createAction("template.searchLeadProtocol", "fas:folder-tree", this.leadProtocol),
            DataTableUtils.createAction("template.sign", "fas:signature", this.sign, (row) => { return row.protocolNumber == null }, false),
            DataTableUtils.createAction("template.executiveSign", "fas:user-pen", this.executiveSign, (row) => { return row.protocolNumber == null }, false),
            DataTableUtils.createAction("template.send", "fas:paper-plane", this.send, (row) => { return row.protocolNumber != null }, false),
            DataTableUtils.createAction("template.checkEmailStatus", "fas:check-to-slot", this.emailStatus, (row) => { return row.protocolNumber != null }, false),
            DataTableUtils.createAction("template.protocol", "fas:stamp", this.protocol, (row) => { return row.protocolNumber == null }, false),
            DataTableUtils.createAction("template.startProcess", "fas:play", this.startProcess, (row) => { return row.protocolNumber != null }, false),
            DataTableUtils.createAction("template.stopProcess", "fas:stop", this.stopProcess, (row) => { return row.protocolNumber != null }, false),
            DataTableUtils.createAction("template.releaseInChargeOfTheFolder", "fas:retweet", this.releaseInChargeOfTheFolder, (row) => { return row.protocolNumber != null }, false),
            DataTableUtils.createAction("common.delete", IconConstants.dataTable.delete, this.delete, (row) => { return row.protocolNumber == null }, false),
            DataTableUtils.createAction('common.download', "fas:download", this.download),
        ];
        this.tableManager = new ListTableManager(this.loadTemplates);
    }

    public executiveSign = (template: TemplateSmall): void => {
        this._licenseesService.documentToSign(this.licensee.id, template.id).subscribe(result => {
            this.showNotification = true;
            this.tableManager.startReload();
        });
    }

    public sendNotification = (): void => {
        this._dialogService.show(SendNotificationComponent, {
            panelClass: "modal-lg",
            data: this.licensee.id,
            callback: result => {
                if (result) {
                    this.tableManager.startReload();
                }
            }
        });
    }

    public startProcess = (template: TemplateSmall): void => {
        this._dialogService.show(ProcessComponent, {
            panelClass: "modal-xl",
            data: { id: template.id, isStarting: true },
            callback: result => {
                if (result) {
                    this.tableManager.startReload();
                }
            }
        });
    }

    public stopProcess = (template: TemplateSmall): void => {
        this._dialogService.show(ProcessComponent, {
            panelClass: "modal-xl",
            data: { id: template.id, isStarting: false },
            callback: result => {
                if (result) {
                    this.tableManager.startReload();
                }
            }
        });
    }

    public leadProtocol = (template: TemplateSmall): void => {
        this._licenseesService.getConnectedLeadProtocol((template.id).toString()).subscribe(result => {
            this._spinnerService.show();
            if (result) {
                this._dialogService.show(ConnectedProtocolComponent, {
                    data: { result: result, displayResult: false }
                });
            }
            else if (template.protocolNumber == null) {
                this._dialogService.show(SearchLeadProtocolComponent, {
                    panelClass: "modal-lg",
                    data: template.id,
                    callback: result => {
                        if (result) {
                            this.tableManager.startReload();
                        }
                    }
                });
            }
            else {
                this._dialogService.show(ConnectedProtocolComponent, {
                    data: { result: result, displayResult: true }
                });
            }
            this._spinnerService.hide();
        });
    }

    public releaseInChargeOfTheFolder = (template: TemplateSmall): void => {
        this._licenseesService.realiseInChargeOfTheFolder(template.id).subscribe(result => {
            this._snackBarService.success("common.successfull");
            this.tableManager.startReload();
        });
    }

    public download = (template: TemplateSmall): void => {
        this._licenseesService.downloadTemplatesByLicenseeId(template.id).subscribe(result => {
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
    }

    public protocol = (template: TemplateSmall): void => {
        this._dialogService.show(ProtocolComponent, {
            panelClass: "modal-lg",
            data: { template: template, send: false },
            callback: result => {
                if (result) {
                    this.tableManager.startReload();
                }
            }
        });
    }

    public sign = (template: TemplateSmall): void => {
        this._dialogService.show(SignDocumentComponent, {
            data: [template.id],
            callback: result => {
                if (result) {
                    this.tableManager.startReload();
                }
            }
        });
    }

    public send = (template: TemplateSmall): void => {
        this._dialogService.show(ProtocolComponent, {
            data: { template: template, send: true, licenseeId: this.licensee.id },
            callback: result => {
                if (result) {
                }
            }
        });
    }

    public emailStatus = (template: TemplateSmall): void => {
        this._dialogService.show(EmailstatusComponent, {
            panelClass: "modal-sm",
            data: template.id,
            callback: result => {

            }
        })
    }

    public delete = (row: TemplateSmall): void => {
        this._dialogService.showConfirm("association.deleteAssociationTitle", "association.deleteAssociatioMessage", {
            callback: (result) => {
                if (result) {
                    this._licenseesService.deleteTemplate(this.licensee.id, row.id).subscribe(result => {
                        this.tableManager.startReload();
                    });
                }
            }
        },
            [row.templateDescription]
        );
    }
}