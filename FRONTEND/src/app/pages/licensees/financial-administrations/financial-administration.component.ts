import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { AutocompleteActionItem, BaseComponent, CommonValidators, DialogService, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { LicenseesService } from 'app/services/licensees.service';
import { PeopleService } from 'app/services/people.service';
import { FinancialAdministration, FinancialAdministrationWrite } from 'app/models/licensees.model';
import { PersonComponent } from 'app/pages/configuration/people/person.component';
import { ChipItem } from '@asf/ng14-library';
import { PersonAutocomplete, PersonTypes } from 'app/models/people.model';


@Component({
    selector: 'financial-administration',
    templateUrl: './financial-administration.component.html',
})
export class FinancialAdministrationComponent extends BaseComponent implements OnInit {
    public form: UntypedFormGroup = null;
    public actionItems: AutocompleteActionItem[] = [];
    public actionLegalItems: AutocompleteActionItem[] = [];
    public drivers: ChipItem<PersonAutocomplete>[] = []
    public isEdit: boolean = false;
    public initialAutocompleteItem: any = null;

    constructor(private _fb: UntypedFormBuilder, private _translateService: TranslateService, private _licenseesService: LicenseesService, private _spinnerService: SpinnerService,
        private _dialogService: DialogService, private _snackBarService: SnackBarService, private _peopleService: PeopleService,
        private _dialogRef: MatDialogRef<FinancialAdministrationComponent>, @Inject(MAT_DIALOG_DATA) private _licenseeId: number) {
        super();
    }

    ngOnInit(): void {

        let people = new AutocompleteActionItem();
        people.callback = this.addUser;
        people.label = this._translateService.instant("person.addPerson");
        people.visible = true;
        this.actionItems.push(people);

        let legal = new AutocompleteActionItem();
        legal.callback = this.addLegalUser;
        legal.label = this._translateService.instant("person.addLegalPerson");
        legal.visible = true;
        this.actionLegalItems.push(legal);

        this._licenseesService.getFinancialAdministration(this._licenseeId).subscribe(result => {
            this.createForm(result);
            if (result) {
                this.initialAutocompleteItem = {
                    id: result.legalPersonId,
                    displayName: result.legalPersonDisplayName,
                }
                this.drivers = result.drivers.map(m => new ChipItem(m));
                this.isEdit = true;
            }
            this._spinnerService.hide();
        });
    }

    public loadAutocompleteLegalItems = (value: string): Observable<PersonAutocomplete[]> => {
        return this._peopleService.people(value, PersonTypes.Legal)
    }

    public loadAutocompleteItems = (value: string): Observable<PersonAutocomplete[]> => {
        return this._licenseesService.taxiDriversAutocomplete(this._licenseeId, value);
    }

    private createForm = (item?: FinancialAdministration): void => {
        item = item || new FinancialAdministration();
        this.form = this._fb.group({
            id: [item.id],
            legalPersonId: [item.legalPersonId, CommonValidators.required],
        });
    }

    public onSelectedItem = (person: PersonAutocomplete): void => {
        this.drivers = [...this.drivers, new ChipItem(person)];
    }

    public removeUser = (person: PersonAutocomplete): void => {
        this.drivers = this.drivers.filter(f => f.data.id != person.id);
    }

    public addLegalUser = (): void => {
        this._dialogService.show(PersonComponent, {
            panelClass: "modal-xl",
            data: {id: null, isLegal: true},
            callback: result => {
                if (result) {
                    this._peopleService.getPersonById(result).subscribe(person => {
                        this.initialAutocompleteItem = {
                            id: person.id,
                            displayName: person.lastName + ", " + person.fiscalCode
                        }
                        this.form.controls.legalPersonId.setValue(result);
                    })
                }
            }
        });
    }

    public addUser = (): void => {
        this._dialogService.show(PersonComponent, {
            panelClass: "modal-xl",
            data: {id: null, isLegal: false},
            callback: result => {
                if (result) {
                    this._peopleService.getPersonById(result).subscribe(person => {
                        const item = new PersonAutocomplete();
                        item.displayName = person.firstName + " " + person.lastName + "," + person.fiscalCode;
                        item.id = person.id;
                        this.drivers = [...this.drivers, new ChipItem(item)];
                        //this.form.controls.personId.setValue(result);
                    })
                }
            }
        });
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            const data = <FinancialAdministrationWrite>this.form.value;
            data.drivers = this.drivers.map(m => m.data);
            this._licenseesService.saveFinancialAdministration(data, this._licenseeId, this.isEdit).subscribe(() => {
                this._snackBarService.success("common.successfull");
                this._dialogRef.close(true);
            });
        }
    }

    public close() {
        this._dialogRef.close(false);
    }
}
