import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { BaseComponent, CommonValidators, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AssociationsService } from 'app/services/associations.service';
import { CategoryAssociation, CategoryAssociationWrite } from 'app/models/category-associations.model';
import { fuseAnimations } from '@asf/ng14-library';


@Component({
    selector: 'association',
    templateUrl: './association.component.html',
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})
export class AssociationComponent extends BaseComponent implements OnInit {
    public form: UntypedFormGroup = null;
    public isNew: boolean = false;

    constructor(private _fb: UntypedFormBuilder, private _associationService: AssociationsService, private _spinnerService: SpinnerService, private _snackBarService: SnackBarService,
        private _dialogRef: MatDialogRef<AssociationComponent>, @Inject(MAT_DIALOG_DATA) private _id?: number) {
        super();
        this.isNew = this._id == null;
    }

    ngOnInit(): void {
        if (this._id) {
            this._spinnerService.show();
            this._associationService.getAssociationsById(this._id).subscribe(result => {
                this.createForm(result)
                this._spinnerService.hide();
            });
        }
        else {
            this.isNew = true;
            this.createForm();
        }
    }

    private createForm = (item: CategoryAssociation = new CategoryAssociation()): void => {
        this.form = this._fb.group({
            name: [item.name, CommonValidators.required],
            fiscalCode: [item.fiscalCode, CommonValidators.required],
            email: [item.email, CommonValidators.required],
            telephoneNumber: [item.telephoneNumber, CommonValidators.required],
        });
    }

    public close() {
        this._dialogRef.close(false);
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            const data = <CategoryAssociationWrite>this.form.getRawValue();
            this._associationService.save(data, this._id).subscribe(result => {
                this._snackBarService.success("common.successfull");
                this._dialogRef.close(true);
                this._spinnerService.hide();
            });
        }
    }
}


