import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BaseComponent, SpinnerService } from '@asf/ng14-library';
import { EmailStatus } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';


@Component({
    selector: 'email-status',
    templateUrl: './email-status.component.html',
})
export class EmailstatusComponent extends BaseComponent implements OnInit {

    public status: EmailStatus;

    constructor(private _spinnerService: SpinnerService, private _licenseesService: LicenseesService,
        private _dialogRef: MatDialogRef<EmailstatusComponent>, @Inject(MAT_DIALOG_DATA) private _data?: number) {
        super();
    }

    ngOnInit(): void {
        this._spinnerService.show();
        this._licenseesService.checkEmailStatus(this._data).subscribe(result => {
            this.status = result;
        })
        this._spinnerService.hide()
    }

    public close() {
        this._dialogRef.close(false);
    }
}