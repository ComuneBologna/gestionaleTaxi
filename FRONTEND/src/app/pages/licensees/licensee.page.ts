import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BaseComponent, RouterUtils, SpinnerService } from '@asf/ng14-library';
import { LicenseeTypes, NccLicensee, TaxiLicensee } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';
import { fuseAnimations } from '@asf/ng14-library';



@Component({
    selector: 'licensee',
    templateUrl: './licensee.page.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class LicenseePage extends BaseComponent implements OnInit {
    private _id: number = null;
    public licensee: TaxiLicensee | NccLicensee;
    public isTaxi: boolean = true;
    public goBackUrl: string = null;

    constructor(private _licenseesService: LicenseesService, private _activatedRoute: ActivatedRoute, private _spinnerService: SpinnerService, private _router: Router) {
        super();
    }

    ngOnInit(): void {
        this.on(this._activatedRoute.params.subscribe(params => {
            this.isTaxi = params["type"] == "taxi";
            localStorage.setItem('type', params['type']);
            //this.goBackUrl = this.isTaxi ? "/licensees/taxi" : "/licensees/ncc";
            if (params['id']) {
                this._spinnerService.show();
                this._id = +params["id"];
                this._licenseesService.getLicenseebyId(this._id, this.isTaxi).subscribe(result => {
                    this._spinnerService.hide();
                    if (result == null) {
                        this.goto404();
                    }
                    this.licensee = result;
                });
            }
            else {
                this.goto404();
            }
        }));
    }

    public addRequest = (): void => {
        this._router.navigateWithReturnUrl(["/", "licensees", this.isTaxi ? "taxi" : "ncc", this._id, "request"], RouterUtils.createReturnUrl(null, null));
    }

    private goto404 = (): void => {
        this._router.navigate(["errors", "404"]);
    }
}
