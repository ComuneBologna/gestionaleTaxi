import { NgModule } from '@angular/core';
import { DashboardComponent } from './dashboard.component';
import { Route, RouterModule } from '@angular/router';
import { CommonComponentsModule, SharedModule } from '@asf/ng14-library';
import { AuthGuard } from 'app/shared/auth.guard';


const routes: Route[] = [
    { path: '', component: DashboardComponent, canActivate: [AuthGuard] }];

@NgModule({
    declarations: [
        DashboardComponent
    ],
    imports: [
        SharedModule,
        CommonComponentsModule,
        RouterModule.forChild(routes)
    ],
    providers: [
    ],
    exports: [
    ]
})
export class DashboardModule {
}
