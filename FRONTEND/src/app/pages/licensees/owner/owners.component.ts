import { Component, Input, ViewEncapsulation } from '@angular/core';
import { BaseComponent, SpinnerService, DialogService } from '@asf/ng14-library';
import { forkJoin } from 'rxjs';
import { fuseAnimations } from '@asf/ng14-library';
import { LicenseeEditOwner, NccLicensee, TaxiDriver, TaxiLicensee } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';
import { OwnerComponent } from './owner.component';

@Component({
    selector: 'owners',
    templateUrl: './owners.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class OwnersComponent extends BaseComponent {
    private _licensee: TaxiLicensee | NccLicensee;
    @Input()
    public get licensee(): TaxiLicensee | NccLicensee {
        return this._licensee;
    }
    public set licensee(value: TaxiLicensee | NccLicensee) {
        this._licensee = value;
        this.loadData();
    }
    public variations: TaxiDriver[] = [];
    public owner: TaxiDriver;
    public showVariations: boolean = false;

    constructor(private _dialogService: DialogService, private _licenseesService: LicenseesService, private _spinnerService: SpinnerService) {
        super();
    }

    private loadData = (): void => {
        this._spinnerService.show();
        forkJoin([this._licenseesService.getOwner(this.licensee.id), this._licenseesService.getOwnersVariations(this.licensee.id)]).subscribe(results => {
            this.owner = results[0];
            this.variations = [...results[1]];
            this._spinnerService.hide();
        });
    }

    public manageOwner(isNew: boolean) {
        this._dialogService.show(OwnerComponent, {
            panelClass: "modal-xl",
            data: new LicenseeEditOwner(this.licensee.id, !isNew),
            callback: result => {
                if (result) {
                    this.loadData();
                }
            }
        });
    }
}