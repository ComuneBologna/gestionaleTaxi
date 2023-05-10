import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SnackBarService, BaseComponent, SpinnerService, SelectListitem, EnumUtils } from '@asf/ng14-library';
import { TranslateService } from '@ngx-translate/core';
import { DigitalSign, DigitalSignCredential, MultipleSign, SignType } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';

@Component({
    selector: 'sign-document',
    templateUrl: './sign-document.component.html',
})
export class SignDocumentComponent extends BaseComponent implements OnInit {

    public form: UntypedFormGroup;
    public showOtp: boolean = false;
    public signTypeItems: SelectListitem[] = [];
    public signType: SignType;

    constructor(private _spinnerService: SpinnerService, private _fb: UntypedFormBuilder, private _licenseesService: LicenseesService, private translateServices: TranslateService,
        private _snackBarService: SnackBarService, private _dialogRef: MatDialogRef<SignDocumentComponent>, @Inject(MAT_DIALOG_DATA) private _data?: any) {
        super();
    }

    ngOnInit(): void {
        this._spinnerService.show();
        this.signTypeItems = EnumUtils.toSelectListitems(SignType, "SignType", this.translateServices);
        this._licenseesService.getCredential().subscribe(result => {
            if (result) {
                let credential = new DigitalSign();
                credential.username = result.username;
                credential.password = result.password;
                this.createForm(credential);
            }
            else {
                this.createForm(new DigitalSign());
            }
        })
        this.signType = SignType.CADES;
        this._spinnerService.hide()
    }

    private createForm = (item: DigitalSign): void => {
        this.form = this._fb.group({
            otp: [item.otp],
            username: [item.username],
            password: [item.password],
        });
    }

    public close() {
        this._dialogRef.close(false);
    }

    public getOtp = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let data = new DigitalSignCredential();
            let form = this.form.getRawValue();
            data.username = form.username;
            data.password = form.password;
            this._licenseesService.sign(data).subscribe(result => {
                this._snackBarService.success("common.successfull");
                this.showOtp = true;
                this._spinnerService.hide();
            });
        }
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let data = new MultipleSign();
            let form = this.form.getRawValue();
            data.credential = new DigitalSign();
            data.credential.otp = form.otp;
            data.credential.password = form.password;
            data.credential.username = form.username;
            data.requestRegisterIds = this._data;
            this._licenseesService.multipleSignWithOtp(this.signType, data).subscribe(result => {
                this._dialogRef.close(true);
                this._snackBarService.success("common.successfull");
                this._spinnerService.hide();
            });
        }
    }

}

