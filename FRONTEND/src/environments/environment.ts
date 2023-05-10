// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
    production: false,
    authentication: {
        issuer: "",
        silentRefreshRedirectUri: "",
        clientId: "",
        responseType: "",
        scope: "",
        postLogoutRedirectUri: "",
        redirectUri: ""
    },
    apiUrl: "",
    coreApiUrl: "",
    usersApiUrl: "",
    profileApiUrl: "",
    avatarApiUrl: "",
    applicationId: 12,
    culture: {
        resourcePaths: [
            "https://webclients.smartpa.cloud/localization/{code}.json",
            "./assets/i18n/app-{code}.json",
            "./assets/i18n/enum-{code}.json"
        ],
        cultureCodes: [
            "it"
        ],
        defaultCultureCode: "it"
    },
    forms: {
        "appearance": "outline",
        "floatLabel": "auto"
    }
};
//apiUrl: 'https://localhost:44342/',


/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
