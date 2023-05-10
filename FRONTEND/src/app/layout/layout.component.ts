

import { Component, ViewEncapsulation } from '@angular/core';
import { defaultNavigation } from 'app/shared/navigation';

@Component({
    selector: 'layout',
    templateUrl: './layout.component.html',
    styleUrls: ['./layout.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class LayoutComponent {
    public navigation = defaultNavigation;
}
