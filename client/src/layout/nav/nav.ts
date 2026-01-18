import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { ToastService } from '../../core/services/toast-service';
import { themes } from '../themes';
import { BusyService } from '../../core/services/busy-service';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav implements OnInit{
  protected accountService =inject(AccountService);
  protected router=inject(Router);
  private toast=inject(ToastService);
  protected busyService=inject(BusyService);
  protected creds : any = {};
  protected selectedTheme = signal<string>(localStorage.getItem('theme') || 'light');
  protected themes= themes;

  ngOnInit(): void {
    //  Apply the theme to the HTML element immediately on load
    document.documentElement.setAttribute('data-theme', this.selectedTheme());
  }

  // Επιλογή θέματος και αποθήκευση στην τοπική αποθήκη
  handleSelectTheme(theme: string) {
    this.selectedTheme.set(theme);
    localStorage.setItem('theme',theme);
    document.documentElement.setAttribute('data-theme', theme);
    const elem= document.activeElement as HTMLElement;
    elem?.blur(); // Απομάκρυνση εστίασης από το κουμπί μετά την επιλογή θέματος
  }

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
