import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdministratorRoutingModule } from './administrator-routing.module';
import { AdminListComponent } from './admin-list/admin-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AuthGuardService } from '../guards/auth-guard.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { Interceptor } from '../_helpers/interceptor';


@NgModule({
  declarations: [
    AdminListComponent
  ],
  imports: [
    CommonModule,
    AdministratorRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    AuthGuardService,
    {provide: HTTP_INTERCEPTORS, useClass: Interceptor, multi: true}
  ],
})
export class AdministratorModule { }
