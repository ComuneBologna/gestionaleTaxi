import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { BaseComponent, SpinnerService, DialogService } from '@asf/ng14-library';
import { forkJoin } from 'rxjs';
import { fuseAnimations } from '@asf/ng14-library';
import { LicenseeEditVehicle, NccLicensee, TaxiLicensee, Vehicle } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';
import { VehicleComponent } from './vehicle.component';


@Component({
    selector: 'vehicles',
    templateUrl: './vehicles.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class VehiclesComponent extends BaseComponent {
    private _licensee: TaxiLicensee | NccLicensee;
    @Input()
    public get licensee(): TaxiLicensee | NccLicensee {
        return this._licensee;
    }
    public set licensee(value: TaxiLicensee | NccLicensee) {
        this._licensee = value;
        this.loadData();
    }
    public variations: Vehicle[] = [];
    public vehicle: Vehicle;
    public showVariations: boolean = false;

    constructor(private _dialogService: DialogService, private _licenseesService: LicenseesService, private _spinnerService: SpinnerService) {
        super();
    }

    private loadData = (): void => {
        this._spinnerService.show();
        forkJoin([this._licenseesService.getVehicle(this.licensee.id), this._licenseesService.getVehiclesVariations(this.licensee.id)]).subscribe(results => {
            this.vehicle = results[0];
            this.variations = [...results[1]];
            this._spinnerService.hide();
        })
    }

    public manageVehicle(isNew: boolean) {
        this._dialogService.show(VehicleComponent, {
            panelClass: "modal-lg",
            data: new LicenseeEditVehicle(this.licensee.id, !isNew),
            callback: result => {
                if (result) {
                    this.loadData();
                }
            }
        });
    }



}