import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AssociationsPage } from '../configuration/category-associations/associations.page';
import { AssociationComponent } from '../configuration/category-associations/association.component';
import { CommonComponentsModule, SharedModule } from '@asf/ng14-library';
import { PeoplePage } from './people/people.page';
import { PersonComponent } from './people/person.component';
import { ShiftsPage } from './shift/shifts.page';
import { ShiftComponent } from './shift/shift.component';
import { IssuingOfficesPage } from './issuing-office/issuing-offices.page';
import { IssuingOfficeComponent } from './issuing-office/issuing-office.component';
import { TemplatesPage } from './templates/templates.page';
import { TemplateComponent } from './templates/template.component';
import { TagsComponent } from './templates/tags.component';
import { EmailPage } from './email/email.page';
import { EditEmailComponent } from './email/edit-email.component';
import { CredentialsPage } from './credentials/credentials.page';
import { MatInputModule } from '@angular/material/input';


const appRoutes: Routes = [
    { path: 'people', component: PeoplePage },
    { path: 'associations', component: AssociationsPage },
    { path: 'shifts', component: ShiftsPage },
    { path: 'issuing-offices', component: IssuingOfficesPage },
    { path: 'templates', component: TemplatesPage },
    { path: 'email', component: EmailPage },
    { path: 'credentials', component: CredentialsPage },
]

@NgModule({
    declarations: [
        PeoplePage,
        PersonComponent,
        AssociationsPage,
        AssociationComponent,
        IssuingOfficesPage,
        IssuingOfficeComponent,
        ShiftsPage,
        ShiftComponent,
        TemplatesPage,
        TemplateComponent,
        TagsComponent,
        EmailPage,
        EditEmailComponent,
        CredentialsPage
    ],
    imports: [
        SharedModule,
        CommonComponentsModule,
        MatInputModule,
        RouterModule.forChild(appRoutes)

    ],
})
export class ConfigurationModule { }
