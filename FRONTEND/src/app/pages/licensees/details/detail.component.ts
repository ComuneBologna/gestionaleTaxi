import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { BaseComponent, RIGHTBAR_DATA, SpinnerService } from '@asf/ng14-library';
import { fuseAnimations } from '@asf/ng14-library';
import { DriverSubstitution, LicenseeItem, LicenseeTypes, NccLicensee, SubstitutionsStatus, TaxiDriver, TaxiLicensee } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';
import { forkJoin, map } from 'rxjs';

@Component({
    selector: 'detail',
    templateUrl: './detail.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None,
})
export class DetailComponent extends BaseComponent implements OnInit {

    public loaded: boolean = false;
    public licensee: TaxiLicensee | NccLicensee;
    public isTaxi: boolean;
    public owner: TaxiDriver;
    public substitution: DriverSubstitution;

    constructor(private _spinnerService: SpinnerService, private _licenseeservices: LicenseesService, private _router: Router,
        @Inject(RIGHTBAR_DATA) public data: LicenseeItem) {
        super();
    }

    ngOnInit(): void {
        this.loaded = false;
        this._spinnerService.show();
        let type = LicenseeTypes;
        this.isTaxi = this.data.type == type.Taxi ? true : false;
        forkJoin([this._licenseeservices.getLicenseebyId(this.data.id, this.isTaxi), this._licenseeservices.getOwner(this.data.id), 
            this._licenseeservices.getSubstitutionsDrivers(this.data.id)]).pipe(map(results => {
                this.licensee = results[0];
                this.owner = results[1];
                this.substitution = results[2].filter(f => f.status != SubstitutionsStatus.ToActivate).first();
        })).subscribe(() => {
            this.loaded = true;
            this._spinnerService.hide();
        })
    }
   
    public edit = (): void => {
        this._router.navigate(["/", "licensees", this.isTaxi ? "taxi" : "ncc", this.data.id]);
    }
}
