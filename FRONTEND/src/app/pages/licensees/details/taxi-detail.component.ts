import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { BaseComponent, SpinnerService, DialogService } from '@asf/ng14-library';
import { fuseAnimations } from '@asf/ng14-library';
import { LicenseeEditData, TaxiLicensee, Variation } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { EditLicenseeComponent } from '../edit-licensee.component';

@Component({
    selector: 'taxi-details',
    templateUrl: './taxi-details.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class TaxiDetailsComponent extends BaseComponent implements OnInit {
    @Input() public licensee: TaxiLicensee;
    @Output() public licenseeChange: EventEmitter<TaxiLicensee> = new EventEmitter<TaxiLicensee>();
    public variations: Variation[] = [];
    public showVariations: boolean = false;

    constructor(private _dialogService: DialogService, private _licenseesService: LicenseesService, private _spinnerService: SpinnerService) {
        super();
    }
    ngOnInit(): void {
        this.loadVariations().subscribe();
    }

    public loadVariations = (): Observable<Variation[]> => {
        return this._licenseesService.getlicenseeVariations(this.licensee.id).pipe(tap(result => {
            this.variations = [...result];
        }))
    }

    public manageLicensee = (isVariation: boolean): void => {
        this._dialogService.show(EditLicenseeComponent, {
            panelClass: "modal-lg",
            data: new LicenseeEditData(true, isVariation, this.licensee.id),
            callback: result => {
                if (result) {
                    if (isVariation) {
                        this.loadVariations().subscribe();
                    }
                    this._licenseesService.getLicenseebyId(this.licensee.id, true).subscribe(result => {
                        this.licenseeChange.emit(<TaxiLicensee>result);
                    });
                }
            }
        });
    }

}
