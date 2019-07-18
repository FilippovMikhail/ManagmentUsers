import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, AbstractControl, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal'
import { ConditionalExpr } from '@angular/compiler';
import { registerLocaleData } from '@angular/common';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styles: []
})
export class RegisterComponent implements OnInit {

  insertForm: FormGroup;
  FirstName: FormControl;
  UserName: FormControl;
  Password: FormControl;
  ConfirmPassword: FormControl;
  Email: FormControl;
  ErrorList: string[];

  constructor(private accountService: AccountService,
    private router: Router,
    private fromBuilder: FormBuilder) { }

  ngOnInit() {
    this.FirstName = new FormControl('', [Validators.required]);
    this.UserName = new FormControl('', [Validators.required]);
    this.Email = new FormControl('', [Validators.required, Validators.email])
    this.Password = new FormControl('', [Validators.required, Validators.minLength(6)])
    this.ConfirmPassword = new FormControl('', [Validators.required, this.MustMatch(this.Password)]);
    this.ErrorList = [];

    this.insertForm = this.fromBuilder.group({
      "FirstName": this.FirstName,
      "UserName": this.UserName,
      "Email": this.Email,
      "Password": this.Password,
      "ConfirmPassword": this.ConfirmPassword
    });
  }

  onSubmit() {
    var user = this.insertForm.value;
//Вызываем серис, который отправит наши данные на сервер
    this.accountService.registration(user.FirstName, user.UserName, user.Email, user.Password)
      .subscribe(result => {
//Переходим на страницу Логин
        this.router.navigate(['/login']);

      },
        error => {
          console.log(error);
        });
  }

  //Проверка паролей
  MustMatch(passwordControl: AbstractControl): ValidatorFn {
    return (confirmPasswordControl: AbstractControl): { [key: string]: boolean } | null => {
      //Возвращаем null, если пароли не совпадают
      if (!passwordControl && !confirmPasswordControl) {
        return null;
      }
      //Возвращаем значение null, если другой валидатор уже обнаружил ошибку
      if (confirmPasswordControl.hasError && !passwordControl.hasError) {
        return null;
      }

      //Устанавливаем ошибку если проверка не пройдена
      if (passwordControl.value !== confirmPasswordControl.value) {
        return { 'MustMatch': true };
      }
      else {
        return null;
      }
    }
  }

}
