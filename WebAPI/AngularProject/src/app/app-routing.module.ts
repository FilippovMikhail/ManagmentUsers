import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';


const routes: Routes = [
  {path: '', component: LoginComponent, pathMatch:'full'}, //Если ничего не ввели, то отображаем компонент по умолчанию
  {path:'subscribers', loadChildren: './subscribers/subscribers.module#SubscribersModule'},
  {path:"registration", component: RegisterComponent},
  {path:"administrator", loadChildren: './administrator/administrator.module#AdministratorModule'},
  {path:'login', component: LoginComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
