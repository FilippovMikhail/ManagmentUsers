import { Injectable } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Role } from '../interfaces/Role';
import { User } from '../interfaces/User';
import { UserAndRole } from '../interfaces/UserAndRole';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  constructor(private http: HttpClient) { }

  private accessPointUrl: string = "http://localhost:50038/api/";
  private listRolesUrl: string = this.accessPointUrl + "administrator/getroles";
  private listUsersUrl: string = this.accessPointUrl + "administrator/getusers";
  private addUserToRoleUrl: string = this.accessPointUrl + "administrator/addusertorole";

  private roles$: Observable<Role[]>;
  private users$: Observable<User[]>;

  //Получаем список ролей, и назначенные им права
  getRoles(): Observable<Role[]> {
    this.roles$ = this.http.get<Role[]>(this.listRolesUrl);
    return this.roles$;
  }

  //Получаем список всех учетных записей пользователей
  getUsers(): Observable<any> {
    this.users$ = this.http.get<any>(this.listUsersUrl);
    return this.users$;
  }
  //Обновление ролей для учетной записи пользователя 
  updateUserToRole(userAndRole: UserAndRole): Observable<UserAndRole> {
    return this.http.post<UserAndRole>(this.addUserToRoleUrl, userAndRole);
  }
}
