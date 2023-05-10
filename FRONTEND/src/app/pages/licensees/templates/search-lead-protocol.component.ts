import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { SelectListitem, SpinnerService, DataTableColumn, DataTableAction, BaseTablePageComponent, IDataTableManager, RemoteDataTableManager, DataTableUtils, EnumUtils, DialogService, SnackBarService } from '@asf/ng14-library';
import { TranslateService } from '@ngx-translate/core';
import { Attachment } from 'app/models/imports.model';
import { ExternalDocument, ExternalDocumentAttachment, ExternalDocumentSearchCriteria, FolderType, LeadProtocolDataInput } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';

@Component({
    selector: 'lead-protocol',
    templateUrl: './search-lead-protocol.component.html',
})
export class SearchLeadProtocolComponent extends BaseTablePageComponent<ExternalDocument, Filters>{

    public attachment: Attachment;
    public metadatas: SelectListitem[] = [];
    public filters: Filters;
    public tableColumns: DataTableColumn[] = [];
    public tableActions: DataTableAction[] = [];
    public typesItem: SelectListitem[] = [];
    public documentName: string;
    public loaded: boolean = false;
    public clickedRow: ExternalDocument;
    public listDocument: ExternalDocumentAttachment[] = [];
    protected autoSearch: boolean = false;
    public consoleTypeItems: SelectListitem[] = [];

    constructor(private _spinnerService: SpinnerService, private _translateService: TranslateService, private _licenseesService: LicenseesService, private _snackBarService: SnackBarService,
        activatedRoute: ActivatedRoute, private _dialogService: DialogService, private _dialogRef: MatDialogRef<SearchLeadProtocolComponent>, @Inject(MAT_DIALOG_DATA) private _data?: number) {
        super(activatedRoute);
    }

    public initialize = (): void => {
        this.filters = new Filters();
        this.consoleTypeItems = EnumUtils.toSelectListitems(ConsoleFolderType, "ConsoleFolderType", this._translateService);
        this.prepareTable();
    }

    protected getDataTableManager = (searchCriteria: ExternalDocumentSearchCriteria): IDataTableManager<ExternalDocument> => {
        return new RemoteDataTableManager(this._licenseesService.searchLeadProtocol, this.setSearchCriteria, searchCriteria);
    }

    private prepareTable = (): void => {
        let columns: DataTableColumn[] = [];
        columns.push(DataTableUtils.createStringColumn("title", "common.title", true));
        this.tableColumns.push(...columns);
    }

    public search = (): void => {
        this._spinnerService.show();
        this.dataTableManager.startSearch();
        this._spinnerService.hide();
    }

    public clearFilters = (): void => {
        this.filters.protocolYear = null;
        this.filters.title = null;
        this.filters.type = ConsoleFolderType.FASCICOLO_TAXI;
        this.filters.protocolNumber = null;
        this.dataTableManager.startSearch();
    }

    public rowClick = (row: ExternalDocument): void => {
        this.loaded = true;
        this.documentName = row.title;
        this.clickedRow = row;
        this._licenseesService.getDocumentByIdConsole(row.id).subscribe(result => {
            this.listDocument = result;
        })
    }

    public close() {
        this._dialogRef.close(false);
    }

    public loadList() {
        this.loaded = !this.loaded;
    }

    public connectDocument = (document: ExternalDocumentAttachment): void => {
        let leadDocument = new LeadProtocolDataInput();
        leadDocument.externalDocumentId = this.clickedRow.id;
        leadDocument.leadDocumentName = document.name;
        leadDocument.number = +document.protocolNumber;
        leadDocument.year = +document.protocolDate.split('-')[0];
        this._dialogService.showConfirm("template.connectLeadProtocolTitle", "template.connectLeadProtocolMessage", {
            callback: (result) => {
                if (result) {
                    this._spinnerService.show();
                    this._licenseesService.connectToLeadDocument(this._data, leadDocument).subscribe(result => {
                        this._snackBarService.success("common.successfull");
                        this._spinnerService.hide();
                        this._dialogRef.close(false);
                    })
                }
            }
        }, [document.name]);
    }
}
class Filters {
    public status: string;
    public protocolYear: string;
    public title: string;
    public type: ConsoleFolderType;
    public cUPCode: string;
    public protocolNumber: number;
}
enum ConsoleFolderType {
    FASCICOLO_GENERICO_2_0 = "FASCICOLO_GENERICO_2_0",
    FASCICOLO_TAXI = "FASCICOLO_TAXI"
}