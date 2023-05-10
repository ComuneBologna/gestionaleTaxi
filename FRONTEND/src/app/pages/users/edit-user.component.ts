import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UsersService } from 'app/services/users.service';
import { TranslateService } from '@ngx-translate/core';
import { BaseComponent, CommonValidators, EnumUtils, SelectListitem, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { PermissionCodes } from 'app/models/common.model';
import { UserWrite } from 'app/models/users.model';


@Component({
    selector: 'page-edit-user',
    templateUrl: './edit-user.component.html',
})
export class EditUserComponent extends BaseComponent implements OnInit {
    public form: UntypedFormGroup = null;
    public isNew: boolean = false;
    public roles: SelectListitem[] = [];
    public drivers: SelectListitem[] = [];


    constructor(private _fb: UntypedFormBuilder, private _spinnerService: SpinnerService, private _snackBarService: SnackBarService, private _translateService: TranslateService, private _usersService: UsersService, private _dialogRef: MatDialogRef<EditUserComponent>, @Inject(MAT_DIALOG_DATA) private _data?: string) {
        super();
    }

    ngOnInit(): void {
        this.roles = [...EnumUtils.toSelectListitems(PermissionCodes, "PermissionCodes", this._translateService)
            .filter(f => f.id != PermissionCodes.Tenant_Admin)];

        if (this._data) {
            this._spinnerService.show();
            this._usersService.getById(this._data).subscribe(result => {
                this.createForm(result);
                this._spinnerService.hide();
            });
        }
        else {
            this.isNew = true;
            this.createForm(new UserWrite());
        }
        this._usersService.getAllDrivers().subscribe(result => {
            this.drivers = result.map(m => new SelectListitem(m.id, m.personDisplayName));
        });
    }

    private createForm = (item: UserWrite): void => {
        this.form = this._fb.group({
            email: [item.email, CommonValidators.required],
            firstName: [item.firstName, CommonValidators.required],
            lastName: [item.lastName, CommonValidators.required],
            fiscalCode: [item.fiscalCode, CommonValidators.required],
            phoneNumber: [item.phoneNumber],
            isEnabled: [item.isEnabled, CommonValidators.requiredIf(() => !this.isNew)],
            permissionCode: [item.permissionCode, CommonValidators.required],
            avatarId: [item.avatarId],
            driverId: [{ value: item.driverId, disabled: item.permissionCode != PermissionCodes.Taxi_Driver }, [CommonValidators.requiredIf(() => this.form.controls.permissionCode.value == PermissionCodes.Taxi_Driver)]]
        });

        this.form.controls.permissionCode.valueChanges.subscribe((newValue: PermissionCodes) => {
            if (newValue == PermissionCodes.Taxi_Driver) {
                if (this.form.controls.driverId.disabled) {
                    this.form.controls.driverId.enable();
                }
            }
            else {
                if (this.form.controls.driverId.enabled) {
                    this.form.controls.driverId.disable();
                    this.form.controls.driverId.setValue(null);
                }
            }
        });

    }
    public close() {
        this._dialogRef.close(false);
    }

    public save = (): void => {
        if (this.form.isValid()) {
            this._spinnerService.show();
            let data = <UserWrite>this.form.getRawValue();
            this._usersService.save(data, this._data).subscribe(result => {
                this._snackBarService.success("common.operationSuccessfull");
                this._dialogRef.close(true);
                this._spinnerService.hide();
            });
        }
    }

}

