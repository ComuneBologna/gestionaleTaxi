import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BaseComponent, CommonValidators, SelectListitem, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { LicenseesService } from 'app/services/licensees.service';
import { map, Observable } from 'rxjs';


@Component({
    selector: 'process',
    templateUrl: './process.component.html',
})
export class ProcessComponent extends BaseComponent implements OnInit {

    public form: UntypedFormGroup;

    constructor(private _spinnerService: SpinnerService, private _licenseesService: LicenseesService, private _fb: UntypedFormBuilder, private _snackBarService: SnackBarService,
        private _dialogRef: MatDialogRef<ProcessComponent>, @Inject(MAT_DIALOG_DATA) public data?: Data) {
        super();
    }

    ngOnInit(): void {
        this._spinnerService.show();
        this.createForm();
        this._spinnerService.hide()
    }

    private createForm = (): void => {
        this.form = this._fb.group({
            processTypeCode: [null, CommonValidators.required],
        });
    }

    public loadAutocompleteItems = (value: string): Observable<SelectListitem[]> => {
        return this._licenseesService.getProcessCode(value).pipe(map(result => {
           return  result.map(m => new SelectListitem(m.code, m.fullTextSearch))
        }))
    };

    public save() {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let data = this.form.getRawValue();
            let typeCode = data.processTypeCode;
            if (this.data.isStarting) {
                this._licenseesService.startProcess(this.data.id, typeCode).subscribe(x => {
                    this._snackBarService.success("common.successfull");
                    this._dialogRef.close(true)
                    this._spinnerService.hide();
                });
            }
            else {
                this._licenseesService.stopProcess(this.data.id, typeCode).subscribe(x => {
                    this._snackBarService.success("common.successfull");
                    this._dialogRef.close(true)
                    this._spinnerService.hide();
                });
            }
        }
    }

    public close() {
        this._dialogRef.close(false);
    }
}
class Data {
    public id: number;
    public isStarting: boolean;
}
