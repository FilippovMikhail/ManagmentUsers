import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/services/admin.service';
import { User } from '../../interfaces/User';
import { FormGroup, FormBuilder, FormArray, FormControl } from '@angular/forms';
import { map } from 'rxjs/operators';
import { Role } from 'src/app/interfaces/Role';
import { UserAndRole } from 'src/app/interfaces/UserAndRole';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-admin-list',
  templateUrl: './admin-list.component.html',
  styles: [],
  providers: [AdminService]
})
export class AdminListComponent implements OnInit {

  form: FormGroup;
  users: User[] = []; //Список пользователей
  selectedUser: User; //Выбранный пользователь для редактирования
  listRoles: string[] = []; // Список ролей
  isNotClickAddRole: boolean = true; //Если не нажали на сохранить при добавлении роли
  userTableMode: boolean = true; //Отображаем таблицу пользователей
  isNotClickChangeRoleUser: boolean = false; //Если не нажали на кнопку изменить роль
  tableRoles: Role[] = []; //Таблица Роли

  constructor(private adminSevice: AdminService,
    private formBuilder: FormBuilder,
    private toastr: ToastrService) { }

  ngOnInit() {
    this.loadUsers();
  }

  //Загрузка пользователей
  loadUsers() {

    this.form = this.formBuilder.group({
      roles: new FormArray([])
    });

    this.adminSevice.getUsers()
      .pipe(
        map(result => {
          this.users = result.usersAndRoles;
          this.listRoles = result.roles;
        })
      )
      .subscribe();
  }

  //Загрузка ролей
  loadRoles() {
    this.adminSevice.getRoles().subscribe((result: Role[]) => this.tableRoles = result,
    error => {
      this.toastr.error(error);
    });
  }

  //Отображаем список ролей в виде checkbox'ов
  addCheckBoxesWithRoles(user: User) {
    //Перебираем каждую роль и создаем новый экземпляр FormControl
    this.listRoles.map((roleName, i) => {
      var control: FormControl;
        control = new FormControl(user.RoleNames.indexOf(roleName) != -1);//Передаем значения для установки ролей
      (this.form.controls.roles as FormArray).push(control);
    });
  }

  //Изменяем роль у пользователя
  changeRoleUser(user: User) {
    this.selectedUser = user;
    this.addCheckBoxesWithRoles(user);
    this.isNotClickAddRole = false;
    this.isNotClickChangeRoleUser = false;
  }

  //Сохраняем роль для пользователя
  save() {
    const selectedRoleNames = this.form.value.roles
      .map((value, index) => value ? this.listRoles[index] : null)
      .filter(value => value !== null);

    //Добаляем роли пользователю
    var userAndRole = new UserAndRole(null, null, null);

    userAndRole.UserName = this.selectedUser.UserName;

    var excludedRoles = this.selectedUser.RoleNames.filter(x => !selectedRoleNames.includes(x));
    userAndRole.ExcludedRoleNames = excludedRoles;

    var includedRoles = selectedRoleNames.filter(x => !this.selectedUser.RoleNames.includes(x));
    userAndRole.IncludedRoleNames = includedRoles;

    //Вызываем сервис по отправки данных на сервер
    this.adminSevice.updateUserToRole(userAndRole).subscribe(result => this.loadUsers(),
    error => {
      this.toastr.error(error);
    });

    (this.form.controls.roles as FormArray).clear(); //очишаем модель
    this.isNotClickAddRole = true;
    this.isNotClickChangeRoleUser = true;
    this.toastr.success("Роль обновлена");
  }

  //Отображаем таблицу с пользователями
  activeUserTableMode() {
    this.userTableMode = true;
  }
  //Отображаем таблицу с ролями
  activeRoleTableMode() {
    this.loadRoles();
    this.userTableMode = false;
  }
  //Отмена
  cancel() {
    (this.form.controls.roles as FormArray).clear();//очишаем модель
    this.isNotClickAddRole = true;
    this.isNotClickChangeRoleUser = true;
  }
}
