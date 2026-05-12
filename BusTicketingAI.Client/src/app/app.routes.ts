import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Trips as CompanyTrips} from './pages/box-office/trips/trips';
import { Trips as PublicTrips} from './pages/trips/trips' 
import { Seats } from './pages/seats/seats';
import { BusCompanies } from './pages/admin/bus-companies/bus-companies';
import { roleGuard } from './core/guards/role-guard';
import { Users } from './pages/admin/users/users';
import { AdminLayout } from './components/layouts/admin-layout/admin-layout';
import { BoxOffice } from './pages/box-office/box-office/box-office';
import { CompanyLayout } from './components/layouts/company-layout/company-layout';
import { Fleet } from './pages/box-office/fleet/fleet';
import { Profile } from './pages/profile/profile';
import { Checkout } from './pages/checkout/checkout';
import { ResetPassword } from './pages/reset-password/reset-password';

export const routes: Routes = [
    { path: 'trips', component: PublicTrips},
    { path: 'trip/:id/seats', component: Seats},
    { path: 'checkout', component: Checkout},
    { path: 'login', component: Login},
    { path: 'register', component: Register},
    { path: 'reset-password', component: ResetPassword},
    { path: 'profile', component: Profile, canActivate: [roleGuard], data: { role: 'Member' }},
    { 
        path: 'admin', 
        component: AdminLayout,
        canActivate: [roleGuard], 
        data: { role: 'Admin'},
        children: [
            { path: 'companies', component: BusCompanies },
            { path: 'users', component: Users },
            { path: '', redirectTo: 'companies', pathMatch: 'full' } 
        ]
    },
    {
        path: 'company',
        component: CompanyLayout,
        canActivate: [roleGuard],
        data: { role: 'CompanyStaff'},
        children: [
            { path: 'box-office', component: BoxOffice },
            { path: 'trips', component: CompanyTrips},
            { path: 'fleet', component: Fleet},
            { path: '', redirectTo: 'box-office', pathMatch: 'full'}
        ]
    },
    { path: '', redirectTo: '/trips', pathMatch: 'full'},
    { path: '**', redirectTo: '/trips'}
];
