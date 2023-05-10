import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonComponentsModule, SharedModule } from '@asf/ng14-library';
import { MultipleSignComponent } from './multiple-sign.component';
import { TemplatesExecutivePage } from './templates-executive.page';


const appRoutes: Routes = [
    { path: '', component: TemplatesExecutivePage },
]

@NgModule({
    declarations: [
        TemplatesExecutivePage,
        MultipleSignComponent
    ],
    imports: [
        SharedModule,
        CommonComponentsModule,
        RouterModule.forChild(appRoutes)

    ],
    providers: [
    ]
})
export class ExecutiveModule { }
