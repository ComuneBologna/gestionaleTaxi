import { Route } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { EmptyLayoutComponent } from '@asf/ng14-library';
import { AuthGuard } from './shared/auth.guard';
export const appRoutes: Route[] = [
    // Redirect empty path to '/example'
    { path: '', pathMatch: 'full', redirectTo: 'redirect' },
    {
        path: '', component: LayoutComponent, canActivate: [AuthGuard], children: [
            { path: 'dashboard', loadChildren: () => import('./pages/dashboard/dashboard.module').then(m => m.DashboardModule) },
            { path: 'authorities', loadChildren: () => import('./shared/authorities-choicer.module').then(m => m.AuthoritiesChoicerModule) },
            { path: 'profile', loadChildren: () => import('./pages/profile/profile.module').then(m => m.ProfileModule) },
            { path: 'imports', loadChildren: () => import('./pages/imports/imports.module').then(m => m.ImportsModule) },
            { path: 'users', loadChildren: () => import('./pages/users/user.module').then(m => m.UserModule) },
            { path: 'licensees', loadChildren: () => import('./pages/licensees/licensees.module').then(m => m.LicenseesModule) },
            { path: 'configuration', loadChildren: () => import('./pages/configuration/configuration.module').then(m => m.ConfigurationModule) },
            { path: 'templates-executive', loadChildren: () => import('./pages/executive/templates-executive.module').then(m => m.ExecutiveModule) },
            { path: 'manage-templates', loadChildren: () => import('./pages/manage-templates/manage-templates.module').then((m) => m.ManageTemplatesModule) },
        ]
    },
    {
        path: '', component: EmptyLayoutComponent, loadChildren: () => import('./pages/common/common.module').then(m => m.CommonPagesModule)
    }
];



