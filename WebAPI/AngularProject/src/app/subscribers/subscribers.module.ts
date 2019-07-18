import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SubscribersRoutingModule } from './subscribers-routing.module';
import { SubscriberListComponent } from './subscriber-list/subscriber-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AuthGuardService } from '../guards/auth-guard.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { Interceptor } from '../_helpers/interceptor';


@NgModule({
  declarations: [
    SubscriberListComponent
  ],
  imports: [
    CommonModule,
    SubscribersRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    AuthGuardService,
    {provide: HTTP_INTERCEPTORS, useClass: Interceptor, multi: true}
  ],
})
export class SubscribersModule { }
