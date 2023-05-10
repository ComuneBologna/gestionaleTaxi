import { Component, OnInit } from '@angular/core';
import { BaseComponent, CommonValidators, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef } from '@angular/material/dialog';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { TemplatesService } from 'app/services/templates.service';
import { TemplateWrite } from 'app/models/templates.models';
import { environment } from 'environments/environment';

@Component({
    selector: 'add-template',
    templateUrl: './template.component.html',
})
export class TemplateComponent extends BaseComponent implements OnInit {
    public form: UntypedFormGroup;
    public uploadAttachmentUrl: string = null;
    public filename: string;

    constructor(private _spinnerService: SpinnerService, private _fb: UntypedFormBuilder, private _templatesService: TemplatesService, private _snackBarService: SnackBarService,
        private _dialogRef: MatDialogRef<TemplateComponent>) {
        super();
    }

    ngOnInit(): void {
        this.uploadAttachmentUrl = `${environment.apiUrl}/templates/upload`;
        this.createForm();
    }

    private createForm = (): void => {
        this.form = this._fb.group({
            description: [null, CommonValidators.required],
            fileId: [null, CommonValidators.required]
        });
    }

    public close = (): void => {
        this._dialogRef.close();
    }

    public onFileSelected = (value: any): void => {
        this.filename = value.file.name;
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let data = <TemplateWrite>this.form.getRawValue();
            this._templatesService.save(data, null).subscribe(result => {
                this._snackBarService.success("common.successfull");
                this._dialogRef.close(result);
                this._spinnerService.hide();
            });
        }
    }
}