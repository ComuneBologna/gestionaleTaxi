import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ExtraOptions, PreloadAllModules, RouterModule } from '@angular/router';
import { LayoutModule } from 'app/layout/layout.module';
import { AppComponent } from 'app/app.component';
import { appRoutes } from 'app/app.routing';
import { HttpClientModule } from '@angular/common/http';
import { OAuthModule } from 'angular-oauth2-oidc';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { StartupModule, AUTHENTICATION_SERVICE_TOKEN, CULTURE_CONFIGURATION_TOKEN, fuseAppConfig } from '@asf/ng14-library';
import { CoreModule } from './shared/core.module';
import { CommonComponentsModule, FuseModule, FuseConfigModule } from '@asf/ng14-library';
import { environment } from '../environments/environment';

const routerConfig: ExtraOptions = {
    scrollPositionRestoration: 'enabled',
    preloadingStrategy: PreloadAllModules,
    relativeLinkResolution: 'legacy'
};

@NgModule({
    declarations: [
        AppComponent,
    ],
    providers: [
        { provide: CULTURE_CONFIGURATION_TOKEN, useValue: environment.culture },
        { provide: AUTHENTICATION_SERVICE_TOKEN, useFactory: () => environment.authentication }
    ],
    imports: [
        MatProgressSpinnerModule,
        HttpClientModule,
        BrowserModule,
        BrowserAnimationsModule,
        CommonComponentsModule.forRoot(environment.forms),
        RouterModule.forRoot(appRoutes, routerConfig),

        StartupModule.forRoot(environment.culture),
        // Fuse
        FuseModule,
        FuseConfigModule.forRoot(fuseAppConfig),


        // Layout
        LayoutModule,

        OAuthModule.forRoot(),

        // Core
        CoreModule.forRoot(),

    ],
    bootstrap: [
        AppComponent
    ]
})
export class AppModule {

}