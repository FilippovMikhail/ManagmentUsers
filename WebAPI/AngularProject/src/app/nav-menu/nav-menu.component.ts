import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountService } from '../services/account.service';
import { SubscriberService } from '../services/subscriber.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styles: []
})
export class NavMenuComponent implements OnInit {

  constructor(private accountService: AccountService,
    private subscriberService: SubscriberService) { }

  ngOnInit() {
    this.LoginStatus$ = this.accountService.isLoggesIn;
    this.UserName$ = this.accountService.currentUserName;
  }

  //Выходим из системы
  onLogout() {
    //Очищаем кеш
    this.subscriberService.clearCache();
    //Очищаем все данные пользователя
    this.accountService.logout();
  }

  LoginStatus$: Observable<boolean>;

  UserName$: Observable<string>;
}
