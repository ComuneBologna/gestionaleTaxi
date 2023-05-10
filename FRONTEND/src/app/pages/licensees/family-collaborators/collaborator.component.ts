import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormArray, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { AutocompleteActionItem, BaseComponent, CommonValidators, DialogService, EnumUtils, SelectListitem, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { LicenseesService } from 'app/services/licensees.service';
import { PeopleService } from 'app/services/people.service';
import { DocumentTypes, FamilyCollaborationTypes, LicenseeEditFamilyCollaborator, TaxiDriver, TaxiDriverDocument, TaxiDriverVariation, TaxiDriverWrite } from 'app/models/licensees.model';
import { PersonComponent } from 'app/pages/configuration/people/person.component';


@Component({
    selector: 'collaborator',
    templateUrl: './collaborator.component.html',
})
export class CollaboratorComponent extends BaseComponent implements OnInit {
    public form: UntypedFormGroup = null;
    public initialAutocompleteItem: any = null;
    public actionItems: AutocompleteActionItem[] = [];
    public documentTypes: SelectListitem[] = [];
    public showNote: boolean;
    public familyCollaborationTypes: SelectListitem[] = [];

    constructor(private _fb: UntypedFormBuilder, private _translateService: TranslateService, private _licenseesService: LicenseesService, private _spinnerService: SpinnerService,
        private _dialogService: DialogService, private _snackBarService: SnackBarService, private _peopleService: PeopleService,
        private _dialogRef: MatDialogRef<CollaboratorComponent>, @Inject(MAT_DIALOG_DATA) public data: LicenseeEditFamilyCollaborator) {
        super();
    }

    ngOnInit(): void {
        this.documentTypes = EnumUtils.toSelectListitems(DocumentTypes, "DocumentTypes", this._translateService);
        this.familyCollaborationTypes = EnumUtils.toSelectListitems(FamilyCollaborationTypes, 'FamilyCollaborationTypes', this._translateService);

        let people = new AutocompleteActionItem();
        people.callback = this.addUser;
        people.label = this._translateService.instant("person.addPerson");
        people.visible = true;
        this.actionItems.push(people);

        this._licenseesService.getFamilyCollaborator(this.data.licenseeId).subscribe(result => {
            this.showNote = !this.data.isEdit && result != null;

            if (this.data.isEdit) {
                this.createForm(result);
                this.initialAutocompleteItem = {
                    id: result.personId,
                    label: result.personDisplayName,
                }
            }
            else {
                this.createForm();
            }
            this._spinnerService.hide();
        });
    }



    public loadAutocompleteItems = (value: string): Observable<SelectListitem[]> => {
        return this._licenseesService.taxiDriversAutocomplete(this.data.licenseeId, value).pipe(map(result =>
            result.map(m => new SelectListitem(m.id, m.displayName))
        ));
    }

    private createForm = (item?: TaxiDriver): void => {
        item = item || new TaxiDriver();
        this.form = this._fb.group({
            id: [item.id],
            personId: [item.personId, CommonValidators.required],
            shiftStarts: [item.shiftStarts],
            collaborationType: [item.collaborationType],
            documents: this.createDocumentForm(item.documents)
        });
        if (this.showNote) {
            this.form.addControl("note", new UntypedFormControl(null, CommonValidators.required))
        }
    }

    public createDocumentForm = (documents: TaxiDriverDocument[]): UntypedFormArray => {
        let formArray = new UntypedFormArray([]);
        for (let i = 0; i < documents.length; i++) {
            formArray.push(this.createDocumentFormItem(documents[i]));
        }
        return formArray
    }

    private createDocumentFormItem = (item?: TaxiDriverDocument): UntypedFormGroup => {
        item = item || new TaxiDriverDocument();
        return this._fb.group({
            id: [item.id ? item.id : 0],
            number: [item.number, CommonValidators.required],
            type: [item.type, CommonValidators.required],
            releasedBy: [item.releasedBy, CommonValidators.required],
            validityDate: [item.validityDate, CommonValidators.required]
        });
    }

    public addItem = (): void => {
        this.documentFormArray.push(this.createDocumentFormItem());
    }

    public deleteItem = (index: number): void => {
        this.documentFormArray.removeAt(index);
    }

    public get documentFormArray() {
        return this.form.get('documents') as UntypedFormArray;
    }

    public addUser = (): void => {
        this._dialogService.show(PersonComponent, {
            panelClass: "modal-xl",
            data: {id:null, isLegal:null},
            callback: result => {
                if (result) {
                    this._peopleService.getPersonById(result).subscribe(person => {
                        this.initialAutocompleteItem = {
                            id: person.id,
                            label: person.firstName + " " + person.lastName + " " + person.fiscalCode
                        }
                        this.form.controls.personId.setValue(result);
                    })
                }
            }
        });
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let obs: Observable<any>;
            if (this.showNote) {
                const data = <TaxiDriverVariation>this.form.value;
                obs = this._licenseesService.editFamilyCollaboratorVariation(data, this.data.licenseeId);
            }
            else {
                const data = <TaxiDriverWrite>this.form.value;
                obs = this._licenseesService.saveFamilyCollaborator(data, this.data.licenseeId, this.data.isEdit)
            }
            obs.subscribe(() => {
                this._snackBarService.success("common.successfull");
                this._dialogRef.close(true);
            });
        }
    }

    public close() {
        this._dialogRef.close(false);
    }
}
