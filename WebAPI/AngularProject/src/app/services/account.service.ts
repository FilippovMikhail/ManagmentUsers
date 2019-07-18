import { Injectable } from '@angular/core';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import{ToastrService} from 'ngx-toastr';


@Injectable({
  providedIn: 'root'
})
export class AccountService {

  //Нужен HttpClient для взаимодействия через HTTP с WebAPI
  constructor(private http: HttpClient, private router: Router,
    private toastr: ToastrService) { }

  private accessPointUrl: string = "http://localhost:50038/api/";

  //Url для достуа к методу Login WebAPI
  private baseUrlLogin: string = this.accessPointUrl + "authentication/login";

  //Url для достуа к методу Registration WebAPI
  private baseUrlRegistration: string = this.accessPointUrl + "authentication/registration";

  //Свойства пользователя
  private loginStatus = new BehaviorSubject<boolean>(false);
  private UserName = new BehaviorSubject<string>(localStorage.getItem('userName'));
  private UserRole = new BehaviorSubject<string>(localStorage.getItem('userRole'));

  //Регистрация
  registration(firstName: string, userName: string, email: string, password: string) {
    return this.http.post<any>(this.baseUrlRegistration, { firstName, userName, email, password }).pipe(map(result => {
      //Регистрация прошла успешно
      this.toastr.success("Регистрация прошла успешно!");
      return result;
    },
      error => {
        this.toastr.success("Ошибка при регистрации!");
        return error;
      }));
  }

  //Вход в систему
  login(userName: string, password: string) {

    //Отправляем запрос на сервер
    //в метод post передаем url, а также логин и пароль
    //С помощью метода pipe() объединяем операторы Observable
    return this.http.post<any>(this.baseUrlLogin, { userName, password }).pipe(
      //map() - Оператор преобразования
      map(result => {
        if (result && result.token) {
          //Сохраняем сведения о пользователе и jwt токене в локальном хранилище, чтобы пользователь мог быть аутентифицирован в системе между обновлением страницы
          this.loginStatus.next(true);
          localStorage.setItem('loginStatus', '1');
          localStorage.setItem('jwt', result.token);
          localStorage.setItem('userName', result.userName);
          localStorage.setItem('userRole', JSON.stringify(result.userRole));
          this.UserName.next(localStorage.getItem('userName'));
          this.UserRole.next(localStorage.getItem('userRole'));
        }
        return result;
      })
    );
  }

  //Выход из системы
  logout() {
    //Удаляем все сведения о пользователе и jwt токен
    this.loginStatus.next(false);
    localStorage.removeItem('jwt');
    localStorage.removeItem('userName');
    localStorage.removeItem('userRole');
    localStorage.setItem('loginStatus', '0');
    //Переходим на страницу входа
    this.router.navigate(['/login']);
  }

  get isLoggesIn() {
    return this.loginStatus.asObservable();
  }

  get currentUserName() {
    return this.UserName.asObservable();
  }
}
