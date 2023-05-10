import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BaseComponent, SnackBarService, SpinnerService } from '@asf/ng14-library';

@Component({
    selector: 'new-mail',
    templateUrl: './new-mail.component.html',
})
export class NewMailComponent extends BaseComponent implements OnInit {
    public mail: string;

    constructor(private _spinnerService: SpinnerService, private _snackBarService: SnackBarService, private _dialogRef: MatDialogRef<NewMailComponent>,
        @Inject(MAT_DIALOG_DATA) public data?: any) {
        super();
    }

    ngOnInit(): void {
        this._spinnerService.show();
        this._spinnerService.hide();
    }

    public save() {
        this._snackBarService.success('common.successfull');
        this._dialogRef.close(this.mail);
    }

    public close() {
        this._dialogRef.close(false);
    }
}
