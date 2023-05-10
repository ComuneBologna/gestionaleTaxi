import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { BaseComponent, CommonValidators, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { fuseAnimations } from '@asf/ng14-library';
import { LicenseesService } from 'app/services/licensees.service';
import { LicenseesIssuingOffice, LicenseesIssuingOfficeWrite } from 'app/models/licensees.model';


@Component({
    selector: 'issuing-office',
    templateUrl: './issuing-office.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})
export class IssuingOfficeComponent extends BaseComponent implements OnInit {
    public form: UntypedFormGroup = null;
    public isNew: boolean = false;

    constructor(private _fb: UntypedFormBuilder, private _licenseesService: LicenseesService, private _spinnerService: SpinnerService, private _snackBarService: SnackBarService,
        private _dialogRef: MatDialogRef<IssuingOfficeComponent>, @Inject(MAT_DIALOG_DATA) private _id?: number) {
        super();
        this.isNew = this._id == null;
    }


    ngOnInit(): void {
        if (this._id) {
            this._spinnerService.show();
            this._licenseesService.getIssuingOfficeById(this._id).subscribe(result => {
                this.createForm(result);
            })
            this._spinnerService.hide();
        }
        else {
            this.createForm();
        }
    }

    private createForm = (item?: LicenseesIssuingOffice): void => {
        item = item || new LicenseesIssuingOffice();
        this.form = this._fb.group({
            description: [item.description, CommonValidators.required],
        });
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            const data = <LicenseesIssuingOfficeWrite>this.form.getRawValue();;
            this._licenseesService.saveIssuingOffice(data, this._id).subscribe(result => {
                this._snackBarService.success("common.successfull");
                this._dialogRef.close(true);
                this._spinnerService.hide();
            });
        }
    }

    public close = (): void => {
        this._dialogRef.close(false);
    }
}


