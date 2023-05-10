import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';
import { BaseComponent, Identity } from '@asf/ng14-library';

@Component({
    selector: 'redirect',
    template: '',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RedirectPage extends BaseComponent implements OnInit {
    constructor(private _router: Router) {
        super()
    }
    ngOnInit(): void {
        if (Identity.isTenantAdmin) {
            this._router.navigate(["/users"])
        }
        else {
            this._router.navigate(["/dashboard"]);
        }

    }
}
