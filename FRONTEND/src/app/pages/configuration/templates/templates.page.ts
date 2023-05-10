import { Component, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DataTableAction, DataTableColumn, RemoteDataTableManager, DataTableUtils, DialogService, BaseTablePageComponent, IDataTableManager, SpinnerService, FilesUtils, IconConstants } from '@asf/ng14-library';
import { Tag, Template, TemplateSearchCriteria } from 'app/models/templates.models';
import { fuseAnimations } from '@asf/ng14-library';
import { Import } from 'app/models/imports.model';

import { TemplatesService } from 'app/services/templates.service';
import { TemplateComponent } from './template.component';
import { TagsComponent } from './tags.component';



@Component({
    selector: 'templates-page',
    templateUrl: './templates.page.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class TemplatesPage extends BaseTablePageComponent<Template, Filters> {
    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];

    constructor(private _dialogService: DialogService, private _templateService: TemplatesService, activatedRoute: ActivatedRoute,
        private _spinnerService: SpinnerService) {
        super(activatedRoute)
        this.filters = new Filters();
    }

    protected initialize = (): void => {
        this.prepareTable();
    }

    protected getDataTableManager = (searchCriteria: TemplateSearchCriteria): IDataTableManager<Template> => {
        return new RemoteDataTableManager(this._templateService.searchTemplates, this.setSearchCriteria, searchCriteria);
    }

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createStringColumn("description", "common.description", true));
        columns.push(DataTableUtils.createStringColumn("filename", "template.templateName", true));
        this.tableColumns.push(...columns);
        this.tableActions.push(DataTableUtils.createAction("common.download", "cloud_download", this.download));
        this.tableActions.push(DataTableUtils.createAction("common.delete", IconConstants.dataTable.delete, this.delete));
    }

    public download = (row: Template): void => {
        this._spinnerService.show();
        this._templateService.downloadTemplate(row.id).subscribe(result => {
            if (result) {
                FilesUtils.createLink(result, "_blank")
            }
            this._spinnerService.hide();
        });
    }

    public showTags = (): void => {
        this._dialogService.show(TagsComponent, {
            panelClass: "modal-xl",
        });
    }


    public add() {
        this._dialogService.show(TemplateComponent, {
            panelClass: "modal-lg",
            callback: result => {
                if (result) {
                    this.dataTableManager.startReload();
                }
            }
        });
    }

    public delete = (row: Import): void => {
        this._dialogService.showConfirm("template.deleteTemplateTitle", "template.deleteTemplateMessage", {
            callback: (result) => {
                if (result) {
                    this._templateService.delete(row.id).subscribe(result => {
                        this.dataTableManager.startReload();
                    });
                }
            }
        }, [row.description]);
    }

    public search = (): void => {
        this.dataTableManager.startSearch();
    }

    public reset = (): void => {
        this.filters.description = null;
        this.filters.fileName = null;
        this.dataTableManager.startSearch();
    }
}
class Filters {
    public description: string;
    public fileName: string;
}