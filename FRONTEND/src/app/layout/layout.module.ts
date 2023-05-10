import { NgModule } from '@angular/core';
import { EmptyLayoutModule, ClassicLayoutModule, UserMenuModule } from '@asf/ng14-library';
import { LayoutComponent } from './layout.component';

const layoutModules = [
    EmptyLayoutModule,
    ClassicLayoutModule
];

@NgModule({
    declarations: [
        LayoutComponent
    ],
    imports: [
        UserMenuModule,
        ...layoutModules
    ],
    exports: [
        LayoutComponent
    ]
})
export class LayoutModule {
}


