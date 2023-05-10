import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { AuthorityChoicerPage } from "@asf/ng14-library";
import { AuthGuard } from "./auth.guard";

const routes: Route[] = [
    { path: '', component: AuthorityChoicerPage, canActivate: [AuthGuard] }
];

@NgModule({
    declarations: [
    ],
    imports: [
        RouterModule.forChild(routes)
    ],
    providers: [
    ],
    exports: [
    ]
})
export class AuthoritiesChoicerModule {
}
