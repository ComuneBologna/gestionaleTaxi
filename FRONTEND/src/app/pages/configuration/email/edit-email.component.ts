import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BaseComponent, CommonValidators, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { fuseAnimations } from '@asf/ng14-library';
import { ProtocolEmail } from 'app/models/email.model';
import { EmailService } from 'app/services/email.service';

@Component({
    selector: 'edit-email',
    templateUrl: './edit-email.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None,
})
export class EditEmailComponent extends BaseComponent implements OnInit {
    public form: UntypedFormGroup = null;
    public isNew: boolean = false;

    constructor(private _spinnerService: SpinnerService, private _emailService: EmailService, private _snackBarService: SnackBarService, private _fb: UntypedFormBuilder,
        private _dialogRef: MatDialogRef<EditEmailComponent>, @Inject(MAT_DIALOG_DATA) private _data?: number) {
        super();
    }

    ngOnInit(): void {
        if (this._data) {
            this._spinnerService.show();
            this._emailService.getEmailById(this._data).subscribe((result) => {
                this.createForm(result);
                this._spinnerService.hide();
            });
        } else {
            this.isNew = true;
            this.createForm(new ProtocolEmail());
        }
    }

    private createForm = (item: ProtocolEmail): void => {
        this.form = this._fb.group({
            description: [item.description],
            email: [item.email, CommonValidators.required],
            active: [item.active]
        });
    };

    public close() {
        this._dialogRef.close(false);
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let data = <ProtocolEmail>this.form.getRawValue();
            this._emailService
                .saveEmail(data, this._data)
                .subscribe((result) => {
                    this._snackBarService.success(
                        'common.operationSuccessfull'
                    );
                    this._dialogRef.close(true);
                    this._spinnerService.hide();
                });
        }
    };
}