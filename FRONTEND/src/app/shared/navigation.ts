
import { FuseNavigationItem, IconConstants } from "@asf/ng14-library";
import { PermissionCodes } from "app/models/common.model";

/* tslint:disable:max-line-length */
export const defaultNavigation: FuseNavigationItem[] = [
    {
        id: 'home',
        title: 'common.home',
        type: 'basic',
        icon: IconConstants.menu.home,
        link: '/dashboard',
        permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator, PermissionCodes.Taxi_Driver],
    },
    {
        id: 'licensee',
        title: 'licensee.licensees',
        type: 'collapsable',
        icon: "fas:taxi",
        permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator],
        children: [
            {
                id: 'licensee',
                title: 'licensee.licenseesTaxi',
                type: 'basic',
                link: '/licensees/taxi',
                permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator],
            },
            {
                id: 'licensee',
                title: 'licensee.licenseesNcc',
                type: 'basic',
                link: '/licensees/ncc',
                permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator],
            }
        ]
    },
    {
        id: 'imports',
        title: 'imports.imports',
        type: 'basic',
        link: '/imports',
        icon: "fas:upload",
        permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator],
    },
    {
        id: 'templates-executive',
        title: 'template.templatesExecutive',
        type: 'basic',
        link: '/templates-executive',
        icon: "fas:signature",
        permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin],
    },
    {
        id: 'manage-templates',
        title: 'template.manageTemplates',
        type: 'basic',
        link: '/manage-templates',
        icon: 'fas:folder-tree',
        permissions: [PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator],
    },
    {
        id: 'configuration',
        title: 'common.configuration',
        type: 'collapsable',
        icon: IconConstants.menu.settings,
        permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator],
        children: [
            {
                id: 'anagraphic',
                title: 'common.anagraphic',
                type: 'basic',
                link: '/configuration/people',
                permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator],
            },
            {
                id: 'associations',
                title: 'common.associations',
                type: 'basic',
                link: '/configuration/associations',
                permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator],
            },
            {
                id: 'shift-config',
                title: 'common.shiftConfig',
                type: 'basic',
                link: '/configuration/shifts',
                permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator],
            },
            {
                id: 'issuing-offices',
                title: 'issuingOffice.issuingOffice',
                type: 'basic',
                link: '/configuration/issuing-offices',
                permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator],
            },
            {
                id: 'email',
                title: 'person.email',
                type: 'basic',
                link: '/configuration/email',
                permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator],
            },
            {
                id: 'templates-config',
                title: 'template.templateConfig',
                type: 'basic',
                link: '/configuration/templates',
                permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator],
            },
            {
                id: 'users',
                title: 'users.users',
                type: 'basic',
                link: '/users',
                permissions: [PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Executive],
            },
            {
                id: 'credentials',
                title: 'template.credentials',
                type: 'basic',
                link: '/configuration/credentials',
                permissions: [PermissionCodes.Taxi_Executive, PermissionCodes.Taxi_Admin, PermissionCodes.Taxi_Operator,]
            }
        ]
    }
];
