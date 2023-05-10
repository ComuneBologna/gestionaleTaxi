import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { BaseComponent, CommonValidators, EnumUtils, SelectListitem, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { forkJoin, Observable, of } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { LicenseeEditData, LicenseeNCCVariationWrite, LicenseeNCCWrite, LicenseesIssuingOffice, LicenseeStateTypes, LicenseeTaxiVariationWrite, LicenseeTaxiWrite, LicenseeTypes, LicenseeWrite, NccLicensee, TaxiLicensee } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';
import { AssociationsService } from 'app/services/associations.service';
import { CategoryAssociation } from 'app/models/category-associations.model';
import { ShiftSmall, SubShiftSmall } from 'app/models/shifts.model';

@Component({
    selector: 'edit-licensee',
    templateUrl: './edit-licensee.component.html',
})
export class EditLicenseeComponent extends BaseComponent implements OnInit {

    public form: UntypedFormGroup = null;
    public isNew: boolean = false;
    public stateItems: SelectListitem[] = [];
    public categoryItems: SelectListitem[] = [];
    public typeItems: SelectListitem[] = [];
    public subTypeItems: SelectListitem[] = [];
    public issuingOfficeItems: SelectListitem[] = [];
    public licenseeType = LicenseeTypes;

    constructor(private _fb: UntypedFormBuilder, private _licenseesService: LicenseesService, private _associationsService: AssociationsService, private _spinnerService: SpinnerService, private _snackBarService: SnackBarService,
        private _translateService: TranslateService, private _dialogRef: MatDialogRef<EditLicenseeComponent>, @Inject(MAT_DIALOG_DATA) public data: LicenseeEditData) {
        super();
        this.isNew = this.data.licenseeId == null;
    }

    ngOnInit(): void {
        this._spinnerService.show();
        this.stateItems = EnumUtils.toSelectListitems(LicenseeStateTypes, "LicenseeStateTypes", this._translateService);
        let obs: Observable<NccLicensee | TaxiLicensee> = null;
        if (!this.data.licenseeId) {
            const licensee = this.data.isTaxi ? new TaxiLicensee() : new NccLicensee();
            licensee.status = LicenseeStateTypes.Created;
            this.stateItems = this.stateItems.filter(f => f.id == LicenseeStateTypes.Created || f.id == LicenseeStateTypes.Released)
            licensee.type = this.data.isTaxi ? LicenseeTypes.Taxi : LicenseeTypes.NCC_Auto
            obs = of(licensee);
        }
        else {
            obs = this._licenseesService.getLicenseebyId(this.data.licenseeId, this.data.isTaxi);
        }
        obs.pipe(mergeMap(result => {
            this.createForm(result);
            const obs: Observable<any>[] = [this.getCategoryAssociations(), this.getIssuingOffices()];
            if (result.type == LicenseeTypes.Taxi) {
                obs.push(this.getShifts());
                if ((result as TaxiLicensee).shiftId) {
                    obs.push(this.getSubShifts((result as TaxiLicensee).shiftId));
                }
            }
            return forkJoin(obs);
        })).subscribe(result => {
            this._spinnerService.hide();
        })

    }

    public getIssuingOffices = (): Observable<LicenseesIssuingOffice[]> => {
        return this._licenseesService.getIssuingOffices().pipe(tap(result => {
            this.issuingOfficeItems = [...result.map(m => new SelectListitem(m.id, m.description))];
        }));
    }

    public getCategoryAssociations = (): Observable<CategoryAssociation[]> => {
        return this._associationsService.getAllAssociations().pipe(tap(result => {
            this.categoryItems = [...result.map(m => new SelectListitem(m.id, m.name))];
        }));
    }

    public getShifts = (): Observable<ShiftSmall[]> => {
        return this._licenseesService.getAllShifts().pipe(tap(result => {
            this.typeItems = [...result.map(m => new SelectListitem(m.id, m.name))];
        }));
    }

    public loadSubShifts = (shiftId: number): void => {
        this.subTypeItems = [];
        if (shiftId != null) {
            this.getSubShifts(shiftId).subscribe();
        }
        else {
            this.form.controls.subShiftId.setValue(null);
        }
    }

    public getSubShifts = (shiftId: number): Observable<SubShiftSmall[]> => {
        return this._licenseesService.getSubShiftsByShiftId(shiftId).pipe(tap(result => {
            this.subTypeItems = [...result.map(m => new SelectListitem(m.id, m.name))];
        }));
    }

    private createForm = (item: NccLicensee | TaxiLicensee): void => {
        this.form = this._fb.group({
            id: [item.id],
            number: [{ value: item.number, disabled: this.data.isVariation }, CommonValidators.required],
            releaseDate: [item.releaseDate, CommonValidators.required],
            licenseesIssuingOfficeId: [{ value: item.licenseesIssuingOfficeId, disabled: this.data.isVariation }, CommonValidators.required],
            activityExpiredOnCause: [item.activityExpiredOnCause],
            status: [item.status, CommonValidators.required],
            isFamilyCollaboration: [item.isFamilyCollaboration],
            type: [item.type, CommonValidators.required],
            taxiDriverAssociationId: [item.taxiDriverAssociationId],
            note: [item.note],
            variationNote: [, CommonValidators.requiredIf(() => this.data.isVariation)],
            shiftId: [(item as TaxiLicensee).shiftId],
            acronym: [item.acronym],
            subShiftId: [(item as TaxiLicensee).subShiftId],
            garageAddress: [ (item as NccLicensee).garageAddress, CommonValidators.requiredIf(() => !this.data.isTaxi)],
            isFinancialAdministration: [(item as NccLicensee).isFinancialAdministration, CommonValidators.requiredIf(() => !this.data.isTaxi)]
        });
        this.on(this.form.controls.isFamilyCollaboration.valueChanges.subscribe(newValue => {
            if (newValue)
                this.form.controls.isFinancialAdministration.setValue(false)
        }));
        this.on(this.form.controls.isFinancialAdministration.valueChanges.subscribe(newValue => {
            if (newValue)
                this.form.controls.isFamilyCollaboration.setValue(false)
        }));
        this.on(this.form.controls.shiftId.valueChanges.subscribe(shiftId => {
            this.loadSubShifts(shiftId)
        }));
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let obs: Observable<number>;
            if (this.data.isVariation) {
                const variation = this.data.isTaxi ? this.getTaxiVariationWrite() : this.getNccVariationWrite();
                obs = this._licenseesService.saveVariation(variation, this.data.licenseeId);
            }
            else {
                const data = this.data.isTaxi ? this.getLicenseeTaxiWrite() : this.getLicenseeNCCWrite();
                obs = this._licenseesService.saveLicensee(data, this.data.licenseeId);
            }
            obs.subscribe(result => {
                this._snackBarService.success("common.successfull");
                this._dialogRef.close(true);
                this._spinnerService.hide();
            })
        }
    }

    private getLicenseeTaxiWrite = (): LicenseeTaxiWrite => {
        const ret = this.getBaseData<LicenseeTaxiWrite>();
        ret.shiftId = this.rawValue.shiftId;
        ret.subShiftId = this.rawValue.subShiftId;
        return ret;
    }
    private getLicenseeNCCWrite = (): LicenseeNCCWrite => {
        const ret = this.getBaseData<LicenseeNCCWrite>();
        ret.garageAddress = this.rawValue.garageAddress;
        ret.isFinancialAdministration = this.rawValue.isFinancialAdministration;
        return ret;
    }

    private getTaxiVariationWrite = (): LicenseeTaxiVariationWrite => {
        const ret = this.getBaseData<LicenseeTaxiVariationWrite>();
        ret.shiftId = this.rawValue.shiftId;
        ret.subShiftId = this.rawValue.subShiftId;
        ret.variationNote = this.rawValue.variationNote;
        return ret;
    }
    private getNccVariationWrite = (): LicenseeNCCVariationWrite => {
        const ret = this.getBaseData<LicenseeNCCVariationWrite>();
        ret.garageAddress = this.rawValue.garageAddress;
        ret.isFinancialAdministration = this.rawValue.isFinancialAdministration;
        ret.variationNote = this.rawValue.variationNote;
        return ret;
    }

    private getBaseData = <T extends LicenseeWrite>(): T => {
        return <T>{
            acronym: this.rawValue.acronym,
            isFamilyCollaboration: this.rawValue.isFamilyCollaboration,
            activityExpiredOnCause: this.rawValue.activityExpiredOnCause,
            licenseesIssuingOfficeId: this.rawValue.licenseesIssuingOfficeId,
            note: this.rawValue.note,
            number: this.rawValue.number,
            releaseDate: this.rawValue.releaseDate,
            status: this.rawValue.status,
            taxiDriverAssociationId: this.rawValue.taxiDriverAssociationId,
            type: this.rawValue.type

        };
    }

    private get rawValue(): any {
        return this.form.getRawValue();
    }


    protected destroy = (): void => {
        //Rimuovo sottoscrizioni
    }
}
