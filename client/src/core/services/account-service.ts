import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);

  private baseUrl=environment.apiUrl;

  register(creds:RegisterCreds){
    console.log(creds);
    var user = this.http.post<User>(this.baseUrl + 'account/register', creds).pipe(
      tap(u => {
        if (u) {
          this.setCurrentUser(u);
        }
      })
    );
    return user;
  }

  login(creds: LoginCreds) {
    var user = this.http.post<User>(this.baseUrl + 'account/login', creds).pipe(
      tap(u => {
        if (u) {
          this.setCurrentUser(u);
        }
      })
    );
    return user;
  }

  logout() {
    localStorage.removeItem("user");
    this.currentUser.set(null);
  }

  setCurrentUser(user:User){
    localStorage.setItem("user", JSON.stringify(user));
    this.currentUser.set(user);
  }
}
