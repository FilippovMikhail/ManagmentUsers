import { Injectable } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Subscriber } from '../subscribers/subscriber';
import { shareReplay } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})

export class SubscriberService {

  constructor(private http: HttpClient) { }

  private accessPointUrl: string = "http://localhost:50038/api/";

  private getSubscribersUrl: string = this.accessPointUrl + "subscribers/getsubscribers";
  private createSubscriberUrl: string = this.accessPointUrl + "subscribers/createsubscriber";
  private updateSubscriberUrl: string = this.accessPointUrl + "subscribers/updatesubscriber/";
  private printSubscribersUrl: string = this.accessPointUrl + "subscribers/printsubscriber";

  private subscriber$: Observable<Subscriber[]>;

  //Запрос к серверу для получения всех абонентов
  getSubscribers(): Observable<Subscriber[]> {
      this.subscriber$ = this.http.get<Subscriber[]>(this.getSubscribersUrl);
    return this.subscriber$;
  }

  //Запрос к серверу для создания абонента
  createSubscriber(newSubscriber: Subscriber) {
    return this.http.post<Subscriber>(this.createSubscriberUrl, newSubscriber);
  }

  //Запрос к серверу для обновления абонентов
  updateSubscriber(updateSubscriber: Subscriber) {
    return this.http.put<Subscriber>(this.updateSubscriberUrl, updateSubscriber);
  }

  //Запрос к серверу для печати абонентов
  printSubscriber(): any {
    return this.http.get<any>(this.printSubscribersUrl);
  }

  //Очищаем кеш
  clearCache() {
    this.subscriber$ = null;
  }
}
