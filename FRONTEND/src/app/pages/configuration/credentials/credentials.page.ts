import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { BaseComponent, CommonValidators, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { DigitalSignCredential } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';

@Component({
    selector: 'credentials',
    templateUrl: './credentials.page.html',
    encapsulation: ViewEncapsulation.None,
})
export class CredentialsPage extends BaseComponent implements OnInit {
    public form: UntypedFormGroup;

    constructor(private _licenseeService: LicenseesService, private _fb: UntypedFormBuilder, private _snackBarService: SnackBarService,
        private _spinnerService: SpinnerService) {
        super();
    }

    ngOnInit(): void {
        this._licenseeService.getCredential().subscribe(result => {
            if (result) {
                this.createForm(result);
            }
            else {
                this.createForm(new DigitalSignCredential());
            }
        })
    }

    private createForm = (item: DigitalSignCredential): void => {
        this.form = this._fb.group({
            username: [item.username, CommonValidators.required],
            password: [item.password, CommonValidators.required],
        });
    };

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let data = <DigitalSignCredential>this.form.getRawValue();
            this._licenseeService.saveCredential(data).subscribe((result) => {
                this._snackBarService.success('common.operationSuccessfull');
                this._spinnerService.hide();
            });
        }
    };

    public delete = (): void => {
        this._spinnerService.show();
        this._licenseeService.deleteCredential().subscribe(result => {
            this.createForm(new DigitalSignCredential());
            this._snackBarService.success('common.operationSuccessfull');
            this._spinnerService.hide();
        })
    }
}
