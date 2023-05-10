import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { BaseComponent, CommonValidators, EnumUtils, SelectListitem, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PeopleService } from 'app/services/people.service';
import { fuseAnimations } from '@asf/ng14-library';
import { Person, PersonTypes } from 'app/models/people.model';
import { TranslateService } from '@ngx-translate/core';


@Component({
    selector: 'person',
    templateUrl: './person.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})
export class PersonComponent extends BaseComponent implements OnInit {
    public form: UntypedFormGroup = null;
    public isNew: boolean = false;
    public personItems: SelectListitem[] = [];
    public isLegal: boolean;

    constructor(private _fb: UntypedFormBuilder, private _peopleService: PeopleService, private _spinnerService: SpinnerService, private _snackBarService: SnackBarService,
        private _translateServices: TranslateService, private _dialogRef: MatDialogRef<PersonComponent>, @Inject(MAT_DIALOG_DATA) private _data?: Data) {
        super();
        this.isNew = this._data.id == null;
    }

    ngOnInit(): void {
        this.personItems = EnumUtils.toSelectListitems(PersonTypes, "PersonTypes", this._translateServices)
        if (this._data.id) {
            this._spinnerService.show();
            this._peopleService.getPersonById(this._data.id).subscribe(result => {
                this.isLegal = result.type == PersonTypes.Legal ? true : false;
                this.createForm(result);
                this._spinnerService.hide();
            });
        }
        else {
            this.isNew = true;
            this.createForm(new Person());
            if(this._data.isLegal){
                this.form.controls.type.setValue(PersonTypes.Legal);
            }
            else {
                this.form.controls.type.setValue(PersonTypes.Physical);
            }
        }
    }

    private createForm = (item: Person): void => {
        this.form = this._fb.group({
            id: [item.id],
            firstName: [item.firstName],
            lastName: [item.lastName, CommonValidators.required],
            fiscalCode: [item.fiscalCode, CommonValidators.required],
            birthDate: [item.birthDate],
            birthCity: [item.birthCity],
            birthProvince: [item.birthProvince],
            phoneNumber: [item.phoneNumber],
            emailOrPEC: [item.emailOrPEC],
            address: [item.address],
            zipCode: [item.zipCode],
            residentCity: [item.residentCity],
            residentProvince: [item.residentProvince],
            type: [{value: item.type, disabled: this._data.id ? true : false}]
        });
        this.on(this.form.controls.type.valueChanges.subscribe(result => {
            this.isLegal = result == PersonTypes.Legal ? true : false;
        }))
    }

    public close() {
        this._dialogRef.close(false);
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let data = <Person>this.form.getRawValue();
            this._peopleService.savePerson(data, this._data.id, data.type).subscribe(result => {
                this._snackBarService.success("common.successfull");
                this._dialogRef.close(result);
                this._spinnerService.hide();
            });
        }
    }
}

class Data {
    public id: number;
    public isLegal: boolean;
}


