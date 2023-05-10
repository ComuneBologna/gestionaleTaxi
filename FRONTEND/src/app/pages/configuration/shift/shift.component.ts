import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { UntypedFormArray, UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { BaseComponent, CommonValidators, EnumUtils, NumberValidators, SelectListitem, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { ShiftsService } from 'app/services/shifts.service';
import { fuseAnimations } from '@asf/ng14-library';
import { DaysOfWeek, Shift, ShiftToWrite, SubShift } from 'app/models/shifts.model';


@Component({
    selector: 'shift',
    templateUrl: './shift.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})
export class ShiftComponent extends BaseComponent implements OnInit {
    public form: UntypedFormGroup = null;
    public isNew: boolean = false;
    public restDayItems: SelectListitem[] = [];

    constructor(private _fb: UntypedFormBuilder, private _shiftsService: ShiftsService, private _spinnerService: SpinnerService, private _snackBarService: SnackBarService,
        private _translateService: TranslateService, private _dialogRef: MatDialogRef<ShiftComponent>, @Inject(MAT_DIALOG_DATA) private _id?: number) {
        super();
        this.isNew = this._id == null;
        this.restDayItems = EnumUtils.toSelectListitems(DaysOfWeek, "DaysOfWeek", this._translateService);
    }

    ngOnInit(): void {
        if (this._id) {
            this._spinnerService.show();
            this._shiftsService.getShiftById(this._id).subscribe(result => {
                this.createForm(result);
                this._spinnerService.hide();
            });
        }
        else {
            this.createForm(new Shift());
        }
    }

    private createForm = (item: Shift): void => {
        this.form = this._fb.group({
            id: [item.id],
            isHandicapMode: [item.isHandicapMode],
            handicapBeforeInHour: [{ value: item.handicapBeforeInHour, disabled: !item.isHandicapMode }, [NumberValidators.maximum(24), CommonValidators.requiredIf(() => this.form.controls.isHandicapMode.value)]],
            handicapAfterInHour: [{ value: item.handicapAfterInHour, disabled: !item.isHandicapMode }, [NumberValidators.maximum(24), CommonValidators.requiredIf(() => this.form.controls.isHandicapMode.value)]],
            breakInHours: [item.breakInHours, [CommonValidators.required, NumberValidators.maximum(24)]],
            breakThresholdActivationInHour: [item.breakThresholdActivationInHour, [CommonValidators.required, NumberValidators.maximum(24)]],
            restDayFrequencyInDays: [item.restDayFrequencyInDays, [CommonValidators.required, NumberValidators.maximum(31)]],
            name: [item.name, CommonValidators.required],
            durationInHour: [item.durationInHour, [CommonValidators.required, NumberValidators.maximum(24)]],
            subShifts: this.createSubShiftForm(item.subShifts.any() ? item.subShifts : [new SubShift()])
        });

        this.on(this.form.controls.isHandicapMode.valueChanges.subscribe(newValue => {
            if (newValue) {
                this.form.controls.handicapBeforeInHour.enable();
                this.form.controls.handicapAfterInHour.enable();
            }
            else {
                this.form.controls.handicapBeforeInHour.disable();
                this.form.controls.handicapAfterInHour.disable();
                this.form.controls.handicapBeforeInHour.setValue(null);
                this.form.controls.handicapAfterInHour.setValue(null);
            }
        }));
    }

    public createSubShiftForm = (subShift: SubShift[]): UntypedFormArray => {
        let formArray = new UntypedFormArray([]);
        for (let i = 0; i < subShift.length; i++) {
            formArray.push(this.createSubShiftFormItem(subShift[i]));
        }
        return formArray
    }

    private createSubShiftFormItem = (item?: SubShift): UntypedFormGroup => {
        item = item || new SubShift();
        return this._fb.group({
            id: [item.id],
            name: [item.name, CommonValidators.required],
            restDay: [item.restDay, CommonValidators.required]
        });
    }

    public addItem = (): void => {
        this.subShiftsFormArray.push(this.createSubShiftFormItem());
    }

    public deleteItem = (index: number): void => {
        this.subShiftsFormArray.removeAt(index);
        if (this.subShiftsFormArray.length == 0) {
            this.addItem();
        }
    }

    public get subShiftsFormArray() {
        return this.form.get('subShifts') as UntypedFormArray;
    }

    public close() {
        this._dialogRef.close(false);
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            const data = <ShiftToWrite>this.form.getRawValue();
            this._shiftsService.save(data, this._id).subscribe(result => {
                this._snackBarService.success("common.successfull");
                this._dialogRef.close(true);
                this._spinnerService.hide();
            });
        }
    }
}


