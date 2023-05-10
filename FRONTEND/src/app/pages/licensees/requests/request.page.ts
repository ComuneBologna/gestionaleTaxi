import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { UntypedFormBuilder, UntypedFormGroup } from "@angular/forms";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { ActivatedRoute } from "@angular/router";
import { BaseComponent, FilesUtils, SnackBarService, SpinnerService } from "@asf/ng14-library";
import { fuseAnimations } from "@asf/ng14-library";
import { NccLicensee, TaxiLicensee } from "app/models/licensees.model";
import { LicenseeTemplateWrite, TemplateBase } from "app/models/templates.models";
import { LicenseesService } from "app/services/licensees.service";
import { TemplatesService } from "app/services/templates.service";
import { forkJoin, mergeMap } from "rxjs";

@Component({
    selector: 'request',
    templateUrl: './request.page.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})

export class RequestPage extends BaseComponent implements OnInit {
    public isTaxi: boolean = true;
    public goBackUrl: string = null;
    public templates: TemplateBase[];
    public form: UntypedFormGroup = null;
    public licensee: TaxiLicensee | NccLicensee;
    public previewImageUrl: SafeResourceUrl;
    public currentTemplate: TemplateBase;
    constructor(private _activatedRoute: ActivatedRoute, private _spinnerService: SpinnerService, private _snackBarService: SnackBarService, private _sanitizer: DomSanitizer, private _fb: UntypedFormBuilder, private _licenseesService: LicenseesService, private _templatesService: TemplatesService) {
        super()

    }
    ngOnInit(): void {
        this.on(this._activatedRoute.params.pipe(mergeMap(params => {
            this.isTaxi = params["type"] == "taxi";
            this.goBackUrl = this.isTaxi ? "/licensees/taxi" : "/licensees/ncc";
            return forkJoin([this._templatesService.getAllTemplates(), this._licenseesService.getLicenseebyId(+params["id"], this.isTaxi)]);
        })).subscribe(results => {
            this.templates = [...results[0]];
            this.licensee = results[1];
            this.createForm();
        }));
    }


    private createForm = (): void => {
        this.form = this._fb.group({
            id: [null],
            year: [null],
            date: [null],
            protocolNumber: [null],
            protocolDate: [null],
            internalProtocol: [null],
            dateFrom: [null],
            dateTo: [null],
            months: [null],
            collaboratorRelationship: [null],
            note: [null],
            freeText: [null],
            day: [null]
        });
    }

    public selectTemplate = (template: TemplateBase): void => {
        this.currentTemplate = template;
        this._templatesService.getTemplatePreview(template.id, this.licensee.id).subscribe(result => {
            if(result){
                this.previewImageUrl = this._sanitizer.bypassSecurityTrustResourceUrl(result);            }

            else {
                this.previewImageUrl = null;
            }
        })
    }

    public upload = (file: File): void => {
        this._spinnerService.show();
        this._templatesService.uploadFile(file, this.currentTemplate.id, this.licensee.id).subscribe(x => {
            this._snackBarService.success("template.correctlyUploaded");
            this._spinnerService.hide();
        });
    }

    // public printAndSave = (): void => {
    //     if (this.form.isValid()) {
    //         this._spinnerService.show();
    //         let data = <LicenseeTemplateWrite>this.form.getRawValue();
    //         this._templatesService.saveAndDownloadTemplate(this.currentTemplate.id, this.licensee.id, data).subscribe(result => {
    //             this._snackBarService.success("template.requestSaved");
    //             this._spinnerService.hide();
    //         });
    //     }
    // }

    public preview = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let data = <LicenseeTemplateWrite>this.form.getRawValue();
            this._templatesService.previewTemplate(this.currentTemplate.id, this.licensee.id, data).subscribe(result => {
                FilesUtils.createLink(result, "_blank");
                this._spinnerService.hide();
            });
        }
    }
}
