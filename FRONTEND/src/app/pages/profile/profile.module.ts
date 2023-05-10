import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { Route, RouterModule } from '@angular/router';
import { CommonComponentsModule, SharedModule, ProfilePage } from '@asf/ng14-library';
import { AuthGuard } from 'app/shared/auth.guard';


const routes: Route[] = [
    { path: '', component: ProfilePage, canActivate: [AuthGuard] }];

@NgModule({
    imports: [
        SharedModule,
        CommonComponentsModule,
        MatIconModule,
        RouterModule.forChild(routes)
    ]
})
export class ProfileModule {
}
