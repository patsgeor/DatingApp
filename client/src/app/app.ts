import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Nav } from "../layout/nav/nav";
import { AccountService } from '../core/services/account-service';
import { Home } from "../features/home/home";
import { User } from '../types/user';
import { lastValueFrom } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [Nav, Home],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private http = inject(HttpClient);
  private accountService = inject(AccountService)
  // same constructor(private http: HttpClient) { }
  protected readonly title = 'Datting App';
  public members = signal<User[]>([]);

  async ngOnInit() {
    this.setCurrentUser();
    this.members.set(await this.getUsers());


  }// end ngOnInit

  async getUsers() {
    try {
      return await lastValueFrom(this.http.get<User[]>('https://localhost:5001/api/members'));
    }
    catch (er) {
      console.log(er);
      throw er;
    }
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
  }// end setCurrentUser
}
