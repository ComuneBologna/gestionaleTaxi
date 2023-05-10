import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { CommonComponentsModule, SharedModule } from '@asf/ng14-library';
import { EditLicenseeComponent } from './edit-licensee.component';
import { LicenseePage } from './licensee.page';
import { LicenseesPage } from './licensees.page';
import { MatStepperModule } from '@angular/material/stepper';
import { TaxiDetailsComponent } from './details/taxi-detail.component';
import { NccDetailsComponent } from './details/ncc-details.component';
import { OwnersComponent } from './owner/owners.component';
import { OwnerComponent } from './owner/owner.component';
import { VehiclesComponent } from './vehicles/vehicles.component';
import { VehicleComponent } from './vehicles/vehicle.component';
import { CollaboratorsComponent } from './family-collaborators/collaborators.component';
import { CollaboratorComponent } from './family-collaborators/collaborator.component';
import { SubstitutionsComponent } from './substitutions/substitutions.component';
import { SubstitutionComponent } from './substitutions/substitution.component';
import { FinancialAdministrationsComponent } from './financial-administrations/financial-administrations.component';
import { FinancialAdministrationComponent } from './financial-administrations/financial-administration.component';
import { TemplatesComponent } from './templates/templates.component';
import { RequestPage } from './requests/request.page';
import { ProtocolComponent } from './templates/protocol.component';
import { SignDocumentComponent } from './templates/sign-document.component';
import { MatInputModule } from '@angular/material/input';
import { SendNotificationComponent } from './templates/send-notification.component';
import { ProcessComponent } from './templates/process.component';
import { SearchLeadProtocolComponent } from './templates/search-lead-protocol.component';
import { ConnectedProtocolComponent } from './templates/connected-protocol.component';
import { EmailstatusComponent } from './templates/email-status.component';
import { DetailComponent } from './details/detail.component';
import { NewMailComponent } from './templates/new-mail.component';

const appRoutes: Routes = [
    { path: ':type', component: LicenseesPage },
    { path: ':type/:id', component: LicenseePage },
    { path: ':type/:id/request', component: RequestPage }
]

@NgModule({
    declarations: [
        LicenseesPage,
        LicenseePage,
        EditLicenseeComponent,
        TaxiDetailsComponent,
        NccDetailsComponent,
        OwnersComponent,
        OwnerComponent,
        VehiclesComponent,
        VehicleComponent,
        CollaboratorsComponent,
        CollaboratorComponent,
        SubstitutionsComponent,
        SubstitutionComponent,
        FinancialAdministrationsComponent,
        FinancialAdministrationComponent,
        TemplatesComponent,
        RequestPage,
        ProtocolComponent,
        SignDocumentComponent,
        SendNotificationComponent,
        ProcessComponent,
        SearchLeadProtocolComponent,
        ConnectedProtocolComponent,
        EmailstatusComponent,
        DetailComponent,
        NewMailComponent
    ],
    imports: [
        SharedModule,
        MatStepperModule,
        CommonComponentsModule,
        MatInputModule,
        RouterModule.forChild(appRoutes)

    ]
})
export class LicenseesModule { }
