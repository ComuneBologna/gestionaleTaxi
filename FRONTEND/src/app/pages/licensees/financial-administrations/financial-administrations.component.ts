import { Component, Input, ViewEncapsulation } from '@angular/core';
import { BaseComponent, SpinnerService, DialogService } from '@asf/ng14-library';
import { fuseAnimations, ChipItem } from '@asf/ng14-library';
import { FinancialAdministration, NccLicensee, TaxiLicensee } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';
import { FinancialAdministrationComponent } from './financial-administration.component';

@Component({
    selector: 'financial-administrations',
    templateUrl: './financial-administrations.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class FinancialAdministrationsComponent extends BaseComponent {
    private _licensee: TaxiLicensee | NccLicensee;
    @Input()
    public get licensee(): TaxiLicensee | NccLicensee {
        return this._licensee;
    }
    public set licensee(value: TaxiLicensee | NccLicensee) {
        this._licensee = value;
        this.loadData();
    }
    public item: FinancialAdministration;
    public drivers: ChipItem<string>[] = []

    constructor(private _dialogService: DialogService, private _licenseesService: LicenseesService, private _spinnerService: SpinnerService) {
        super();
    }

    public loadData = (): void => {
        this._spinnerService.show();
        this._licenseesService.getFinancialAdministration(this.licensee.id).subscribe(result => {
            if (result) {
                this.item = result;
                let chipName = result.drivers.map(m => new ChipItem(m.displayName));
                this.drivers = chipName;
                this._spinnerService.hide();
            }
        });
        this._spinnerService.hide();
    }

    public manage = (): void => {
        this._dialogService.show(FinancialAdministrationComponent, {
            panelClass: "modal-xl",
            data: this.licensee.id,
            callback: result => {
                if (result) {
                    this.loadData();
                }
            }
        });
    }

}
