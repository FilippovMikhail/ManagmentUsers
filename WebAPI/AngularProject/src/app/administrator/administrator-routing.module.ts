import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminListComponent } from './admin-list/admin-list.component';
import { AuthGuardService } from '../guards/auth-guard.service';


const routes: Routes = [
  {path:'', component: AdminListComponent, canActivate: [AuthGuardService]},
  {path:'administrator', component: AdminListComponent, canActivate: [AuthGuardService]},
  {path:':id', component: AdminListComponent, canActivate: [AuthGuardService]}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers:[
    AuthGuardService
  ]
})
export class AdministratorRoutingModule { }
