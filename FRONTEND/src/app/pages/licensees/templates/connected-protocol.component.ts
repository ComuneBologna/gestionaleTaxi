import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BaseComponent, SpinnerService } from '@asf/ng14-library';
import { TranslateService } from '@ngx-translate/core';
import { LeadProtocolResult } from 'app/models/licensees.model';


@Component({
    selector: 'connected-protocol',
    templateUrl: './connected-protocol.component.html',
})
export class ConnectedProtocolComponent extends BaseComponent implements OnInit {

    public connectedProtocol: LeadProtocolResult;
    public displayResult: string;

    constructor(private _spinnerService: SpinnerService, private _translateService: TranslateService, private _dialogRef: MatDialogRef<ConnectedProtocolComponent>, @Inject(MAT_DIALOG_DATA) public data: Data) {
        super();
    }

    ngOnInit(): void {
        this._spinnerService.show();
        this.connectedProtocol = this.data.result;
        if (this.data.displayResult) {
            this.displayResult = this._translateService.instant("template.displayResult");
        }
        this._spinnerService.hide()
    }


    public close() {
        this._dialogRef.close(false);
    }
}
class Data {
    public result: LeadProtocolResult;
    public displayResult: boolean;
}