import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { Route, RouterModule } from '@angular/router';
import { CommonComponentsModule, SharedModule } from '@asf/ng14-library';
import { AuthGuard } from 'app/shared/auth.guard';
import { EditUserComponent } from './edit-user.component';
import { UsersPage } from './users.page';


const routes: Route[] = [
    { path: '', component: UsersPage, canActivate: [AuthGuard] }];

@NgModule({
    declarations: [
        UsersPage,
        EditUserComponent
    ],
    imports: [
        SharedModule,
        CommonComponentsModule,
        MatIconModule,
        RouterModule.forChild(routes)
    ],
    providers: [
    ],
    exports: [
        EditUserComponent
    ]
})
export class UserModule {
}
