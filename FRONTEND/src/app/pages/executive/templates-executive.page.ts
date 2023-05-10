import { Component, ViewEncapsulation } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DataTableAction, DataTableColumn, RemoteDataTableManager, DataTableUtils, DialogService, BaseTablePageComponent, IDataTableManager, SnackBarService, ProfileService } from '@asf/ng14-library'; import { CategoryAssociation, CategoryAssociationSearchCriteria } from 'app/models/category-associations.model';
import { fuseAnimations } from '@asf/ng14-library';
import { TemplateSearchCriteria } from 'app/models/templates.models';
import { SignDocumentComponent } from '../licensees/templates/sign-document.component';
import { LicenseesService } from 'app/services/licensees.service';
import { TemplateExecutive } from 'app/models/licensees.model';
import { MultipleSignService } from 'app/services/multiple-sign.service';
import { MultipleSignComponent } from './multiple-sign.component';
import { PermissionCodes } from 'app/models/common.model';


@Component({
    selector: 'templates-executive-page',
    templateUrl: './templates-executive.page.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class TemplatesExecutivePage extends BaseTablePageComponent<TemplateExecutive, Filters> {

    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];
    public form: UntypedFormGroup;
    public canManage: boolean;

    constructor(private _dialogService: DialogService, private _licenseesService: LicenseesService, private _profileService: ProfileService,
        private _multipleSignServices: MultipleSignService, private _snackBarService: SnackBarService, activatedRoute: ActivatedRoute) {
        super(activatedRoute)
        this.filters = new Filters();
    }

    public initialize = (): void => {
        this._profileService.getPermissions().subscribe(result => {
            this.canManage = result.contains(PermissionCodes.Taxi_Executive)
        })
        this.prepareTable();
    }

    protected getDataTableManager = (searchCriteria: TemplateSearchCriteria): IDataTableManager<TemplateExecutive> => {
        return new RemoteDataTableManager(this._licenseesService.searchDocumentsToSign, this.setSearchCriteria, searchCriteria);
    }

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createStringColumn("templateDescription", "common.description", true));
        columns.push(DataTableUtils.createStringColumn("templateFileName", "template.templateName", true));
        columns.push(DataTableUtils.createDateColumn("lastUpdate", "requests.lastUpdate", true));
        columns.push(DataTableUtils.createStringColumn('licenseeNumber', 'licensee.licenseeNumber', true));
        columns.push(DataTableUtils.createEnumColumn('licenseeType', 'licensee.licenseeType', 'LicenseeTypes', true));
        columns.push(DataTableUtils.createIconNoPropertyTableColumn("template.selectedForSign", this.getSelectedIcon));
        this.tableColumns.push(...columns);
        this.tableActions.push(DataTableUtils.createAction("template.sign", "fas:signature", this.sign, () => this.canManage));
        this.tableActions.push(DataTableUtils.createAction("template.addToSign", "far:circle-check", this.addToSelected, (row) => { return !this.isSelected(row) && this.canManage }, false));
        this.tableActions.push(DataTableUtils.createAction("template.removeFromSign", "far:circle", this.removeFromSelected, (row) => { return this.isSelected(row) && this.canManage }, false));
        this.tableActions.push(DataTableUtils.createAction("template.verify", "fas:certificate", this.verify));
        this.tableActions.push(DataTableUtils.createAction('common.download', 'fas:download', this.download));
        this.tableActions.push(DataTableUtils.createAction('template.rollback', 'fas:rotate-left', this.remove));
    }

    public download = (template: TemplateExecutive): void => {
        this._licenseesService.downloadTemplatesByLicenseeId(template.id).subscribe((result) => {
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

    public getSelectedIcon = (row: TemplateExecutive): string => {
        return this.isSelected(row) ? 'far:circle-check' : 'far:circle';
    }

    public isSelected = (row: TemplateExecutive): boolean => {
        return this._multipleSignServices.exists(row.id);
    }

    public addToSelected = (row: TemplateExecutive): void => {
        if (this._multipleSignServices.add(row)) {
            this.updateItems();
        }
        this._snackBarService.success("template.addedToSigned");
    }

    public removeFromSelected = (row: TemplateExecutive): void => {
        if (this._multipleSignServices.remove(row)) {
            this.updateItems();
        }
        this._snackBarService.success("template.removedToSigned");
    }

    public selected = (): void => {
        this._dialogService.show(MultipleSignComponent, {
            data: null,
            callback: result => {
                if (result)
                    this.dataTableManager.startReload();
            }
        });
    }

    public remove = (row: TemplateExecutive): void => {
        this._licenseesService.removeDocumentToSing(row.id).subscribe(result => {
            this._snackBarService.success('common.operationSuccessfull')
            this.dataTableManager.startReload();
        })
    }

    public verify = (row: TemplateExecutive): void => {
        this._licenseesService.verifySignError(row.id).subscribe(result => {
            if (result) {
                let title = 'template.signError'
                this._dialogService.showConfirm(title, result, {
                    callback: result => {

                    }
                })
            }
        })
    }

    private updateItems = (): void => {
        let items = this.dataTableManager.getCurrentItems() as TemplateExecutive[];
        this.dataTableManager.updateCurrentItems(items);
    }

    public sign = (template: TemplateExecutive): void => {
        this._dialogService.show(SignDocumentComponent, {
            data: [template.id],
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

    public clearFilters = (): void => {
        this.filters.description = null;
        this.dataTableManager.startSearch();
    }

}
class Filters {
    public description: string = null;
}