import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AccountService } from '../services/account.service';
import { take, map } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

  constructor(private accountService: AccountService,
    private router: Router,
    private toastr: ToastrService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.accountService.isLoggesIn.pipe(take(1), map((loginStatus: boolean) => {

      const destination: string = state.url;

      //Если пользователь не залогинился, то пренаправляем его на страницу Логина 
      if (!loginStatus) {
        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });

        return false;
      }
      //Берем из хранилища роль пользователя
      var roles = JSON.parse(localStorage.getItem('userRole')).Value;
      //Если пользователь уже вошел в систему, и делает запрос к компоненту, который нуждается в авторизации, то 
      //то мы проверяем, совпадает ли роль для доступа к компоненту с ролью, которая есть у пользователя
      //Если роли совпадают, то пользователь может получить доступ к анному компоненту
      switch (destination) {
        //Если пользователь запрашивал такую страницу
        case '/subscribers/getsubscribers':
          //То нам нужно проверить, действительно ли пользователь имеет нужную роль для доступа к странице
          {
            if (roles.indexOf('Operator') != -1 ||
              roles.indexOf('Manager') != -1 ||
              roles.indexOf('User') != -1) {
              return true;
            }
          }
          break;
        //Если пользователь запрашивал такую страницу
        case '/subscribers/createsubscriber':
          //То нам нужно проверить, действительно ли пользователь имеет нужную роль для доступа к странице
          {
            if (roles.indexOf('Manager') != -1) {
              return true;
            }
          }
          break;
        //Если пользователь запрашивал такую страницу
        case '/subscribers/updatesubscriber':
          //То нам нужно проверить, действительно ли пользователь имеет нужную роль для доступа к странице
          {
            if (roles.indexOf('Operator') != -1 ||
              roles.indexOf('Manager') != -1) {
              return true;
            }
          }
          break;
        //Если пользователь запрашивал такую страницу
        case '/subscribers/printsubscriber':
          //То нам нужно проверить, действительно ли пользователь имеет нужную роль для доступа к странице
          {
            if (roles.indexOf('User') != -1) {
              return true;
            }
          }
          break;
        //Если администратор запрашивал такую страницу
        case '/administrator/getusers':
          //То нам нужно проверить, действительно ли пользователь имеет нужную роль для доступа к странице
          {
            if (roles.indexOf('Admin') != -1) {
              return true;
            }
          }
          break;
        default:
          this.toastr.error("У Вас нет прав для доступа к данному ресурсу. Обратитесь к администратору за помощью");
          return false;
      }
      this.toastr.error("У Вас нет прав для доступа к данному ресурсу. Обратитесь к администратору за помощью");
    }));
  }
}
