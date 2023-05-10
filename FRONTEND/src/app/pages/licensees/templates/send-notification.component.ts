import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BaseComponent, CommonValidators, DialogService, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { ChipItem } from '@asf/ng14-library';
import { RequestRegisterSend, TemplateExecutiveBase } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';


@Component({
    selector: 'send-notification',
    templateUrl: './send-notification.component.html',
})
export class SendNotificationComponent extends BaseComponent implements OnInit {

    public form: UntypedFormGroup;
    public templates: TemplateExecutiveBase[];
    public templatesSelected: ChipItem<TemplateExecutiveBase>[] = []

    constructor(private _spinnerService: SpinnerService, private _licenseesService: LicenseesService, private _fb: UntypedFormBuilder,
        private _snackBarService: SnackBarService, private _dialogRef: MatDialogRef<SendNotificationComponent>, @Inject(MAT_DIALOG_DATA) public data?: number) {
        super();
    }

    ngOnInit(): void {
        this._spinnerService.show();
        this._licenseesService.getDocumentsExecutive(this.data).subscribe(result => {
            this.templates = [...result];
            this.createForm();
        });
        this._spinnerService.hide()
    }

    private createForm = (): void => {
        this.form = this._fb.group({
            executiveEmail: [[], CommonValidators.required],
            ids: [null, CommonValidators.required],
        });
    }

    public onSelectedItem = (template: TemplateExecutiveBase): void => {
        let ids = this.templatesSelected.map(m => m.data.id);
        if (!ids.contains(template.id)) {
            this.templatesSelected = [...this.templatesSelected, new ChipItem(template)];
        }
    }

    public removeUser = (template: TemplateExecutiveBase): void => {
        this.templatesSelected = this.templatesSelected.filter(f => f.data.id != template.id);
    }

    public save() {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let data = <RequestRegisterSend>this.form.getRawValue();
            data.ids = this.templatesSelected.map(m => m.data.id);
            this._licenseesService.sendNotificationToExecutive(this.data, data).subscribe(x => {
                this._snackBarService.success("common.successfull");
                this._dialogRef.close(true);
                this._spinnerService.hide();
            });
        }
    }

    public close() {
        this._dialogRef.close(false);
    }

}

