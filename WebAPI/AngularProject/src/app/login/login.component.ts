import { Component, OnInit } from '@angular/core';
import { AccountService } from '../services/account.service';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import{ToastrService} from 'ngx-toastr';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  insertForm: FormGroup;
  UserName: FormControl;
  Password: FormControl;
  invalidLogin: boolean;
  constructor(private accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute,
    private fromBuilder: FormBuilder,
    private toastr: ToastrService) { }

  onSubmit() {
    var userLogin = this.insertForm.value;
    this.accountService.login(userLogin.UserName, userLogin.Password)
      .subscribe(result => {
        var token = (<any>result).token;
        this.invalidLogin = false;
        if (result.userName == "Administrator") {
          //Переходим на страницу администратора
          this.router.navigate(['/administrator/getusers']);
        }
        else {
          this.router.navigate(['/subscribers/getsubscribers']);
        }
      },
        error => {
          this.invalidLogin = true;
          //Выводим, что введенные даные не верны. Пользователь не найден
          this.toastr.error("Неверные пользователь или пароль");
        });
  }

  ngOnInit() {
    this.UserName = new FormControl('', [Validators.required]);
    this.Password = new FormControl('', [Validators.required])

    this.insertForm = this.fromBuilder.group({
      "UserName": this.UserName,
      "Password": this.Password
    });
  }

}
