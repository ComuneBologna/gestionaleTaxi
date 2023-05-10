import { Component, OnInit } from '@angular/core';
import { BaseComponent, CommonValidators, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef } from '@angular/material/dialog';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { ImportService } from 'app/services/imports.service';
import { ImportWrite } from 'app/models/imports.model';
import { environment } from 'environments/environment';
import { Observable, of } from 'rxjs';


@Component({
    selector: 'import',
    templateUrl: './import.component.html',
})
export class ImportComponent extends BaseComponent implements OnInit {
    public form: UntypedFormGroup;
    public uploadAttachmentUrl: string = null;
    constructor(private _spinnerService: SpinnerService, private _fb: UntypedFormBuilder, private _importService: ImportService, private _snackBarService: SnackBarService,
        private _dialogRef: MatDialogRef<ImportComponent>) {
        super();
    }

    ngOnInit(): void {
        this.uploadAttachmentUrl = `${environment.apiUrl}/imports/upload`
        this.createForm();
    }

    private createForm = (): void => {
        this.form = this._fb.group({
            description: [null, CommonValidators.required],
            attachmentId: [null, CommonValidators.required]
        });
    }

    public close = (): void => {
        this._dialogRef.close();
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            const data = <ImportWrite>this.form.getRawValue();
            this._importService.save(data).subscribe(result => {
                this._snackBarService.success("common.successfull");
                this._dialogRef.close(result);
                this._spinnerService.hide();
            });
        }
    }
    public download = (id: string): Observable<string> => {
        return of(null);
    }

}

