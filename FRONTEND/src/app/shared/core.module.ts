import { APP_INITIALIZER, ModuleWithProviders, NgModule, Optional, SkipSelf } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { MatIconRegistry } from '@angular/material/icon';
import localeIt from '@angular/common/locales/it';
import localeEn from '@angular/common/locales/en';
import { registerLocaleData } from '@angular/common';
import { AUTHENTICATION_SERVICE_TOKEN, HttpService, HTTP_SERVICE_TOKEN, IAuthenticationConfiguration, IHttpServiceConfiguration, ProfileService, ProfileUrlConfiguration, PROFILE_URL_TOKEN, AuthenticationService } from '@asf/ng14-library';
import { AuthGuard } from './auth.guard';
import { OAuthService } from 'angular-oauth2-oidc';
import { HttpClient } from '@angular/common/http';
import { environment } from 'environments/environment';
import { filter, mergeMap, share } from 'rxjs/operators';
import { merge } from 'rxjs';
import { UsersService } from 'app/services/users.service';
import { ImportService } from 'app/services/imports.service';
import { AssociationsService } from 'app/services/associations.service';
import { PeopleService } from 'app/services/people.service';
import { ShiftsService } from 'app/services/shifts.service';
import { LicenseesService } from 'app/services/licensees.service';
import { TemplatesService } from 'app/services/templates.service';
import { EmailService } from 'app/services/email.service';
import { MultipleSignService } from 'app/services/multiple-sign.service';
registerLocaleData(localeIt);
registerLocaleData(localeEn);



export function loadAuthenticationConfiguration(authService: AuthenticationService, profileService: ProfileService) {
    const baseObs = authService.verifyLogin().pipe(share());
    const authenticatedObs = baseObs.pipe(filter(result => result === true))
        .pipe(mergeMap(() => {
            return profileService.initialize();
        }))
    const unAuthenticatedObs = baseObs.pipe(filter(result => result !== true));

    const obs = merge(authenticatedObs, unAuthenticatedObs)
    return () => obs.toPromise();
}

export function initializeHttpServiceToken(authService: AuthenticationService) {
    return <IHttpServiceConfiguration>{
        authenticationService: authService,
        errorUrl: 'errors'
    };
}


@NgModule()
export class CoreModule {
    public static forRoot(): ModuleWithProviders<CoreModule> {
        return {
            ngModule: CoreModule,

            providers: [
                ProfileService,
                UsersService,
                ImportService,
                AssociationsService,
                PeopleService,
                ShiftsService,
                LicenseesService,
                TemplatesService,
                EmailService,
                HttpService,
                AuthenticationService,
                MultipleSignService,
                { provide: HTTP_SERVICE_TOKEN, useFactory: initializeHttpServiceToken, deps: [AuthenticationService] },
                AuthGuard,

                {
                    provide: APP_INITIALIZER,
                    useFactory: loadAuthenticationConfiguration,
                    deps: [AuthenticationService, ProfileService],
                    multi: true
                },
                {
                    provide: PROFILE_URL_TOKEN, useFactory: () => {
                        return <ProfileUrlConfiguration>{
                            profileUrl: environment.profileApiUrl,
                            avatarUrl: environment.avatarApiUrl
                        }
                    }
                }
            ]
        };
    }
    constructor(private _domSanitizer: DomSanitizer, private _matIconRegistry: MatIconRegistry, @Optional() @SkipSelf() parentModule?: CoreModule) {
        // Do not allow multiple injections
        if (parentModule) {
            throw new Error('CoreModule has already been loaded. Import this module in the AppModule only.');
        }

        // Register icon sets
        this._matIconRegistry.registerIcons(this._domSanitizer);
    }
}

