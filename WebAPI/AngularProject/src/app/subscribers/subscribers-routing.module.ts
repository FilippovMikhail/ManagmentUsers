import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SubscriberListComponent } from './subscriber-list/subscriber-list.component';
import { AuthGuardService } from '../guards/auth-guard.service';


const routes: Routes = [
  { path: '', component: SubscriberListComponent , canActivate: [AuthGuardService]},
  { path: 'subscribers', component: SubscriberListComponent, canActivate: [AuthGuardService] },
  { path: ':id', component: SubscriberListComponent, canActivate: [AuthGuardService]}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers:[
    AuthGuardService
  ]
})
export class SubscribersRoutingModule { }
