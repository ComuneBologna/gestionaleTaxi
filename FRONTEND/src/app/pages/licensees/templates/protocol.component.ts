import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AutocompleteActionItem, BaseComponent, ChipItem, CommonValidators, DialogService, SelectListitem, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { TranslateService } from '@ngx-translate/core';
import { ProtocolInfo, ProtocolMail, ProtocolWrite, TemplateSmall } from 'app/models/licensees.model';
import { EmailService } from 'app/services/email.service';
import { LicenseesService } from 'app/services/licensees.service';
import { map, Observable } from 'rxjs';
import { NewMailComponent } from './new-mail.component';


@Component({
    selector: 'protocol',
    templateUrl: './protocol.component.html',
})
export class ProtocolComponent extends BaseComponent implements OnInit {

    public form: UntypedFormGroup;
    public emailItems: SelectListitem[] = [];
    public protocol: ProtocolInfo;
    public recipientsAutocompleteItems: SelectListitem[] = [];
    public recipientsSelected: ChipItem<string>[] = [];
    public actionItems: AutocompleteActionItem[] = [];

    constructor(private _spinnerService: SpinnerService, private _licenseesService: LicenseesService, private _fb: UntypedFormBuilder, private _dialogService: DialogService,
        private _translateService: TranslateService, private _emailService: EmailService, private _snackBarService: SnackBarService,
        private _dialogRef: MatDialogRef<ProtocolComponent>, @Inject(MAT_DIALOG_DATA) public data?: Protocol) {
        super();
    }

    ngOnInit(): void {
        this._spinnerService.show();
        let actionAutocomplete = new AutocompleteActionItem();
        actionAutocomplete.callback = this.newMail;
        actionAutocomplete.label = this._translateService.instant('email.addEmail');
        actionAutocomplete.visible = true;
        this.actionItems.push(actionAutocomplete);
        if (this.data.send) {
            this._licenseesService.recipientsList(this.data.licenseeId).subscribe((result) => {
                this.recipientsAutocompleteItems = [...result.map(m => new SelectListitem(m, m))];
            });
            this.getEmail().subscribe();
            this.createSendForm(new ProtocolMail());
        }
        else {
            this.createWriteForm(new ProtocolWrite());
        }
        this.form.controls.subject.setValue(this.data.template.templateDescription);
        this.data.send;
        this._spinnerService.hide()
    }

    private createWriteForm = (itemWrite: ProtocolWrite): void => {
        this.form = this._fb.group({
            subject: [itemWrite.subject, CommonValidators.required],
            recipient: [itemWrite.recipient, CommonValidators.required],
        });
    }

    public createSendForm = (item: ProtocolMail): void => {
        this.form = this._fb.group({
            subject: [item.subject, CommonValidators.required],
            sender: [item.sender, CommonValidators.required],
            text: [item.text, CommonValidators.required],
            recipients: [null, CommonValidators.required],
        });
    }

    public newMail = (): void => {
        this._dialogService.show(NewMailComponent, {
            panelClass: 'modal-sm',
            data: null,
            callback: (result) => {
                if (result) {
                    this.recipientsSelected = [...this.recipientsSelected, new ChipItem(result)];
                }
            },
        });
    }

    public getEmail = (): Observable<SelectListitem[]> => {
        return this._emailService.getEmail().pipe(map(result => {
            return this.emailItems = result.map(m => new SelectListitem(m.email, m.email))
        }))
    }

    public onSelectedItem = (recipient: SelectListitem): void => {
        let ids = this.recipientsSelected.map((m) => m.data);
        if (!ids.contains(recipient.id)) {
            this.recipientsSelected = [...this.recipientsSelected, new ChipItem(recipient.id)];
        }
    };

    public recipientsAutocomplete = (): void => {
        this._licenseesService.recipientsList(this.data.licenseeId).subscribe((result) => {
            this.recipientsAutocompleteItems = [...result.map(m => new SelectListitem(m, m))];
        });
    };

    public removeUser = (recipient: string): void => {
        this.recipientsSelected = this.recipientsSelected.filter((f) => f.data != recipient);
    };

    public save() {
        if (this.form.isValid()) {
            if (!this.data.send) {
                this._spinnerService.show();
                let data = <ProtocolWrite>this.form.getRawValue();
                this._licenseesService.protocol(this.data.template.id, data).subscribe(result => {
                    if (result) {
                        this.protocol = result;
                        this._spinnerService.hide();
                    }
                });
            }
            else {
                this._spinnerService.show();
                let data = new ProtocolMail();
                let form = this.form.getRawValue();
                data.sender = form.sender;
                data.subject = form.subject;
                data.text = form.text;
                data.recipients = this.recipientsSelected.map(m => m.data);
                this._licenseesService.sendProtocol(this.data.template.id, data).subscribe(result => {
                    this._snackBarService.success("common.successfull");
                    this._dialogRef.close(true)
                    this._spinnerService.hide();
                })
            }
        }
    }

    public close() {
        this._dialogRef.close(true);
    }

}
class Protocol {
    public template: TemplateSmall;
    public send: boolean;
    public licenseeId: number;
} 
