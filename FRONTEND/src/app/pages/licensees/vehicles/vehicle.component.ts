import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { BaseComponent, CommonValidators, DialogService, EnumUtils, SelectListitem, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { LicenseesService } from 'app/services/licensees.service';
import { CarFuelTypes, LicenseeEditVehicle, Vehicle, VehicleVariation, VehicleWrite } from 'app/models/licensees.model';

@Component({
    selector: 'vehicle',
    templateUrl: './vehicle.component.html',
})
export class VehicleComponent extends BaseComponent implements OnInit {
    public form: UntypedFormGroup = null;
    public carFuelTypeItems: SelectListitem[] = [];
    public showNote: boolean;

    constructor(private _fb: UntypedFormBuilder, private _translateService: TranslateService, private _licenseesService: LicenseesService, private _spinnerService: SpinnerService,
        private _dialogService: DialogService, private _snackBarService: SnackBarService,
        private _dialogRef: MatDialogRef<VehicleComponent>, @Inject(MAT_DIALOG_DATA) public data: LicenseeEditVehicle) {
        super();
    }

    ngOnInit(): void {
        this.carFuelTypeItems = EnumUtils.toSelectListitems(CarFuelTypes, "CarFuelTypes", this._translateService);

        this._licenseesService.getVehicle(this.data.licenseeId).subscribe(result => {
            this.showNote = !this.data.isEdit && result != null;

            if (this.data.isEdit) {
                this.createForm(result);
            }
            else {
                this.createForm();
            }
            this._spinnerService.hide();
        });
    }

    private createForm = (item?: Vehicle): void => {
        item = item || new Vehicle();
        this.form = this._fb.group({
            id: [item.id],
            model: [item.model, CommonValidators.required],
            licensePlate: [item.licensePlate, CommonValidators.required],
            carFuelType: [item.carFuelType],
            registrationDate: [item.registrationDate, CommonValidators.required],
            inUseSince: [item.inUseSince, CommonValidators.required],
        });
        if (this.showNote) {
            this.form.addControl("note", new UntypedFormControl(null, CommonValidators.required))
        }
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let obs: Observable<any>;
            if (this.showNote) {
                const data = <VehicleVariation>this.form.value;
                obs = this._licenseesService.saveVehicleVariation(data, this.data.licenseeId);
            }
            else {
                const data = <VehicleWrite>this.form.value;
                obs = this._licenseesService.saveVehicle(data, this.data.licenseeId, this.data.isEdit)
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
