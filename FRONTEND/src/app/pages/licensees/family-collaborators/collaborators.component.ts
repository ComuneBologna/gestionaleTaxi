import { Component, Input, ViewEncapsulation } from '@angular/core';
import { BaseComponent, SpinnerService, DialogService } from '@asf/ng14-library';
import { forkJoin } from 'rxjs';
import { fuseAnimations } from '@asf/ng14-library';
import { LicenseeEditFamilyCollaborator, NccLicensee, TaxiDriver, TaxiLicensee } from 'app/models/licensees.model';
import { LicenseesService } from 'app/services/licensees.service';
import { CollaboratorComponent } from './collaborator.component';


@Component({
    selector: 'collaborators',
    templateUrl: './collaborators.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class CollaboratorsComponent extends BaseComponent {
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
    public collaborator: TaxiDriver;
    public showVariations: boolean = false;

    constructor(private _dialogService: DialogService, private _licenseesService: LicenseesService, private _spinnerService: SpinnerService) {
        super();
    }

    private loadData = (): void => {
        this._spinnerService.show();
        forkJoin([this._licenseesService.getFamilyCollaborator(this.licensee.id), this._licenseesService.getFamilyCollaboratorVariations(this.licensee.id)]).subscribe(results => {
            this.collaborator = results[0];
            this.variations = [...results[1]];
            this._spinnerService.hide();
        });
    }

    public manageFamilyCollaborator(isNew: boolean) {
        this._dialogService.show(CollaboratorComponent, {
            panelClass: "modal-xl",
            data: new LicenseeEditFamilyCollaborator(this.licensee.id, !isNew),
            callback: result => {
                if (result) {
                    this.loadData();
                }
            }
        });
    }
}