import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { BaseComponent, SpinnerService } from '@asf/ng14-library';

@Component({
    selector: 'home',
    templateUrl: './dashboard.component.html',
    encapsulation: ViewEncapsulation.None
})
export class DashboardComponent extends BaseComponent implements OnInit {
    public active: number;
    public credit: number;
    public debit: number;
    public averagepoints: string;

    constructor(private _spinner: SpinnerService) {
        super()
    }
    ngOnInit(): void {
    }


}
