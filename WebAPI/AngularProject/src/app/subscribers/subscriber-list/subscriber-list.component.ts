import { Component, OnInit, ViewChild } from '@angular/core';
import { SubscriberService } from 'src/app/services/subscriber.service';
import { Subscriber } from '../subscriber';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Observable, Subject } from 'rxjs';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-subscriber-list',
  templateUrl: './subscriber-list.component.html',
  styleUrls: ['./subscriber-list.component.css'],
  providers: [SubscriberService]
})
export class SubscriberListComponent implements OnInit {

  //Добавление абонента
  insertForm: FormGroup;
  INN: FormControl;
  ShortName: FormControl;
  FullName: FormControl;
  Address: FormControl;
  Phones: FormControl;
  FIOHead: FormControl;
  SubscriberRepresentative: FormControl;
  RepresentativePhones: FormControl;

  subscriber: Subscriber; //Абонент, который будет добавляться или редактироваться
  subscribers: Subscriber[] = []; //Хранит все полученные с сервера данные
  tableMode: boolean = true; //Значение, показывающее находимся ли мы в процессе добавления объекта или в процессе просмотра объектов

  roles: any;

  constructor(private subscriberService: SubscriberService,
    private fromBuilder: FormBuilder,
    private router: Router,
    private toastr: ToastrService) { }

  //Начальная загрузка данных
  ngOnInit() {
    this.roles = JSON.parse(localStorage.getItem('userRole')).Value;
    this.loadSubscribers();

    //Форма добавления данных
    //Проверка полей
    this.INN = new FormControl('', [Validators.required]);
    this.ShortName = new FormControl('', [Validators.required]);
    this.FullName = new FormControl('', [Validators.required]);
    this.Address = new FormControl('', [Validators.required]);
    this.Phones = new FormControl('', [Validators.required]);
    this.FIOHead = new FormControl('', [Validators.required]);
    this.SubscriberRepresentative = new FormControl('', [Validators.required]);
    this.RepresentativePhones = new FormControl('', [Validators.required]);

    this.insertForm = this.fromBuilder.group({
      "INN": this.INN,
      "ShortName": this.ShortName,
      "FullName": this.FullName,
      "Address": this.Address,
      "Phones": this.Phones,
      "FIOHead": this.FIOHead,
      "SubscriberRepresentative": this.SubscriberRepresentative,
      "RepresentativePhones": this.RepresentativePhones
    });
  }

  //Получение данных через сервис
  loadSubscribers() {
    this.subscriberService.getSubscribers().subscribe((data: Subscriber[]) => this.subscribers = data,
    error => {
      this.toastr.error(error);
    });
  }

  //Сохранение нового объекта
  save() {
    var sub = this.insertForm.value;
    this.subscriberService.createSubscriber(sub)
      .subscribe(data => {
        this.toastr.success("Новый абонент добавлен");
        this.tableMode = true;
        this.insertForm.reset();
        this.loadSubscribers();},
        error => {
          this.toastr.error(error);
        });
  }

  //Сохренение обновленных данных
  saveUpdate() {
    var isUpdate = false;
    this.subscriberService.updateSubscriber(this.subscriber)
      .subscribe(data => {
        this.toastr.success("Данные абонента обнолены");
        this.subscriber = null;
        this.loadSubscribers();
      },
      error => {
        this.toastr.error(error);
      });
  }

  //Отмена при обновлении
  cancelUpdate() {
    this.subscriber = null;
  }

  create() {
    this.tableMode = false; //Скрываем таблицу
  }

  //Обновление абонента
  update(sub: Subscriber) {
    this.subscriber = Object.assign({}, sub);
  }

  //Отмена
  cancel() {
    this.insertForm.reset();
    this.tableMode = true; //Отображаем таблицу
  }

  //Печать списка
  print() {
    this.toastr.warning("Подождите, идет формирование печатной формы");
    var byte: [] = [];
    this.subscriberService.printSubscriber().subscribe(result => {
      byte = result;

      function base64ToArrayBuffer(base64) {
        var binaryString = window.atob(base64);
        var binaryLen = binaryString.length;
        var bytes = new Uint8Array(binaryLen);
        for (var i = 0; i < binaryLen; i++) {
          var ascii = binaryString.charCodeAt(i);
          bytes[i] = ascii;
        }
        return bytes;
      }
      var sampleBytes = base64ToArrayBuffer(byte);
      const a = document.createElement('a');
      document.body.appendChild(a);
      a.style.display = 'none';

      const blob = new Blob([sampleBytes], { type: "octet/stream" });
      const url = window.URL.createObjectURL(blob);
      a.href = url;
      a.download = "Subscribers.pdf";
      a.click();
      window.URL.revokeObjectURL(url);
    },
    error => {
      this.toastr.error(error);
    });

  }

  //В зависимости от роли, узнаем, можно ли создавать нового абонента
  CanCreate(): boolean {
    if (this.roles.indexOf('Manager') != -1) {
      return true;
    }
  }

  //Может ли пользователь изменять абонента
  CanEdit(): boolean {
    if (this.roles.indexOf('Operator') != -1 ||
      this.roles.indexOf('Manager') != -1) {
      return true;
    }
  }

  //Может ли пользователь просматривать список
  CanShow(): boolean {
    if (this.roles.indexOf('Operator') != -1 ||
      this.roles.indexOf('Manager') != -1 ||
      this.roles.indexOf('User') != -1) {
      return true;
    }
  }

  //Может ли пользователь печатать список абонентов
  CanPrint(): boolean {
    if (this.roles.indexOf('User') != -1) {
      return true;
    }
  }
}
