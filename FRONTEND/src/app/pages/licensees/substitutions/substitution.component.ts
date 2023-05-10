import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { AutocompleteActionItem, BaseComponent, CommonValidators, DialogService, SelectListitem, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { LicenseesService } from 'app/services/licensees.service';
import { PeopleService } from 'app/services/people.service';
import { DriverSubstitution, DriverSubstitutionWrite, LicenseeSubstitution, SubstitutionsStatus } from 'app/models/licensees.model';
import { PersonComponent } from 'app/pages/configuration/people/person.component';


@Component({
    selector: 'substitution',
    templateUrl: './substitution.component.html',
})
export class SubstitutionComponent extends BaseComponent implements OnInit {
    public form: UntypedFormGroup = null;
    public initialAutocompleteItem: any = null;
    public actionItems: AutocompleteActionItem[] = [];
    public status: SubstitutionsStatus;

    constructor(private _fb: UntypedFormBuilder, private _translateService: TranslateService, private _licenseesService: LicenseesService, private _spinnerService: SpinnerService,
        private _dialogService: DialogService, private _snackBarService: SnackBarService, private _peopleService: PeopleService,
        private _dialogRef: MatDialogRef<SubstitutionComponent>, @Inject(MAT_DIALOG_DATA) public data: LicenseeSubstitution) {
        super();
    }

    ngOnInit(): void {

        let people = new AutocompleteActionItem();
        people.callback = this.addUser;
        people.label = this._translateService.instant("person.addPerson");
        people.visible = true;
        this.actionItems.push(people);
        if (this.data.id) {
            this._spinnerService.show();
            this._licenseesService.getSubstitutionsDriverById(this.data.licenseeId, this.data.id).subscribe(result => {
                this.status = result.status;
                this.createForm(result);
                this.initialAutocompleteItem = {
                    id: result.substituteDriver.driverId,
                    label: result.substituteDriver.personDisplayName
                };
                this._spinnerService.hide();
            });
        }
        else {
            this.createForm();
        }

    }



    public loadAutocompleteItems = (value: string): Observable<SelectListitem[]> => {
        return this._licenseesService.taxiDriversAutocomplete(this.data.licenseeId, value).pipe(map(result =>
            result.map(m => new SelectListitem(m.id, m.displayName))
        ));
    }

    private createForm = (item?: DriverSubstitution): void => {
        item = item || new DriverSubstitution();
        this.form = this._fb.group({
            id: [item.id],
            substituteDriverId: [item.substituteDriverId, CommonValidators.required],
            startDate: [item.startDate, CommonValidators.required],
            endDate: [item.endDate, CommonValidators.required],
            note: [item.note],
        });
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
            const data = <DriverSubstitutionWrite>this.form.value;
            data.status = this.status;
            this._licenseesService.saveDriverSubstitution(data, this.data.licenseeId, this.data.id).subscribe(() => {
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
