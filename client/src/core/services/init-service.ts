import { inject, Injectable } from '@angular/core';
import { of } from 'rxjs';
import { AccountService } from './account-service';

@Injectable({
  providedIn: 'root'
})
export class InitService {
  private accountService=inject(AccountService);

  init(){
    const userString = localStorage.getItem('user');
    if (userString) {
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
    }
    return of(null);// απαιτεί να επιστρέφει observable<any> | Promise<any> 
  }
}
