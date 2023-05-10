import { Component, Inject, OnInit } from '@angular/core';
import { BaseComponent, DialogService, FilesUtils, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MultipleSignService } from 'app/services/multiple-sign.service';
import { TemplateExecutive } from 'app/models/licensees.model';
import { TemplatesService } from 'app/services/templates.service';
import { SignDocumentComponent } from '../licensees/templates/sign-document.component';


@Component({
    selector: 'multiple-sign',
    templateUrl: './multiple-sign.component.html',
})
export class MultipleSignComponent extends BaseComponent implements OnInit {
    public items: TemplateExecutive[] = [];

    constructor(private _multipleSignService: MultipleSignService, private _spinnerService: SpinnerService, private _dialogService: DialogService, 
        private _snackBarService: SnackBarService, private _dialogRef: MatDialogRef<MultipleSignComponent>, @Inject(MAT_DIALOG_DATA) private _data?: any) {
        super();
    }

    ngOnInit(): void {
        this._spinnerService.show();
        this.items = this._multipleSignService.getAll();
        this._spinnerService.hide();
    }

    public sign = (): void => {
        this._dialogService.show(SignDocumentComponent, {
            data: this.items.map(m => m.id),
            callback: result => {
                if (result) {
                    this.deleteAll();
                    this._snackBarService.success("template.signedDocument");
                }
            }
        });
    }

    public remove = (row: TemplateExecutive): void => {
        this._multipleSignService.remove(row);
        this.items = this._multipleSignService.getAll();
    }

    public deleteAll = (): void => {
        this._multipleSignService.clear();
        this.items = [];
    }

    public close() {
        this._dialogRef.close(this.items);
    }
}
