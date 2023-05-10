import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonComponentsModule, SharedModule } from '@asf/ng14-library';
import { ManageTemplatesPage } from './manage-templates.page';

const appRoutes: Routes = [{ path: '', component: ManageTemplatesPage }];

@NgModule({
    declarations: [
        ManageTemplatesPage,
    ],
    imports: [
        SharedModule,
        CommonComponentsModule,
        RouterModule.forChild(appRoutes),
    ],
    providers: [],
})
export class ManageTemplatesModule { }
