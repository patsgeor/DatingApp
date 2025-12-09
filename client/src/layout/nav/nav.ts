import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { ToastService } from '../../core/services/toast-service';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  protected accountService =inject(AccountService);
  protected router=inject(Router);
  private toast=inject(ToastService);
  protected creds : any = {};

  login(){
    console.log("login...");
    this.accountService.login(this.creds)
      .subscribe({
      next: res => {
        console.log(res); 
        this.toast.success('Logged in successfully');
        this.router.navigateByUrl('/members');// Μετά το login, πλοηγείται στη σελίδα μελών
        this.creds={};// Καθαρίζει τα credentials μετά το login
      },
      error: err => {
        console.log(err);
        this.toast.error(err.error);
      },
      complete: () => console.log('Completed login request')
    });
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }



  
}
