import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';

@Component({
  selector: 'app-nav',
  imports: [FormsModule],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  protected accountService =inject(AccountService);
  protected creds : any = {};

  login(){
    console.log("login...");
    this.accountService.login(this.creds)
      .subscribe({
      next: res => {
        console.log(res); 
        this.creds={};
      },
      error: err => console.log(err),
      complete: () => console.log('Completed')
    });
  }

  logout(){
    this.accountService.logout();
  }



  
}
