import { Component, Inject, OnInit } from '@angular/core';
import { BaseComponent, CommonValidators, SelectListitem, SnackBarService, SpinnerService } from '@asf/ng14-library';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { TemplatesService } from 'app/services/templates.service';
import { Tag } from 'app/models/templates.models';
import { ChipItem } from '@asf/ng14-library';

@Component({
    selector: 'tags',
    templateUrl: './tags.component.html',
})
export class TagsComponent extends BaseComponent implements OnInit {
    public tags: ChipItem<string>[] = [];
    constructor(private _spinnerService: SpinnerService, private _templatesService: TemplatesService,
        private _dialogRef: MatDialogRef<TagsComponent>) {
        super();
    }

    ngOnInit(): void {
        this._templatesService.getAllTags().subscribe(result => {
            this.tags = [...result.map(m => new ChipItem(`${m.description}:${m.value}`))];
        })
    }

    public close = (): void => {
        this._dialogRef.close();
    }

}

