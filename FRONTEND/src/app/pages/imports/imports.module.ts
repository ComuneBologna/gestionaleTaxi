import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonComponentsModule, SharedModule } from '@asf/ng14-library';
import { ImportComponent } from './import.component';
import { ImportsPage } from './imports.page';


const appRoutes: Routes = [
    { path: '', component: ImportsPage },
]

@NgModule({
    declarations: [
        ImportsPage,
        ImportComponent
    ],
    imports: [
        SharedModule,
        CommonComponentsModule,
        RouterModule.forChild(appRoutes)

    ],
    providers: [
    ]
})
export class ImportsModule { }
