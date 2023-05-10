import { NgModule } from '@angular/core';
import { Route, RouterModule } from '@angular/router';
import { CommonComponentsModule, SharedModule, ErrorsPage } from '@asf/ng14-library';
import { RedirectPage } from './redirect/redirect.page';

const routes: Route[] = [
    { path: 'redirect', component: RedirectPage },
    { path: 'errors', component: ErrorsPage },
    { path: 'errors/:code', component: ErrorsPage }
];

@NgModule({
    declarations: [
        RedirectPage
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
export class CommonPagesModule {
}
