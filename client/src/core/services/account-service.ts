import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from './likes-service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  private likesService=inject(LikesService);
  private  refreshTokenIntervalId: any;
  currentUser = signal<User | null>(null);

  private baseUrl=environment.apiUrl;

  register(creds:RegisterCreds){
    console.log(creds);
    return this.http.post<User>(this.baseUrl + 'account/register', creds, {withCredentials:true})
    .pipe(
      tap(u => {
        if (u) {
          this.setCurrentUser(u);
          this.startTokenRefreshInterval();

        }
      })
    );
  }

  login(creds: LoginCreds) {
    return this.http.post<User>(this.baseUrl + 'account/login', creds, {withCredentials:true})
    .pipe(
      tap(u => {
        if (u) {
          this.setCurrentUser(u);
          this.startTokenRefreshInterval();
        }
      })
    );
  }


  refreshToken() {
    return this.http.post<User>(this.baseUrl + 'account/refresh-token', {}, {withCredentials:true});
  }

  startTokenRefreshInterval() {
    this.stopTokenRefreshInterval(); // Stop any existing interval before starting a new one

    this.refreshTokenIntervalId = setInterval(() => { // αποθηκευση του ID του interval για να μπορεί να καθαριστεί αργότερα
      this.http.post<User>(this.baseUrl + 'account/refresh-token', {}, {withCredentials:true}).subscribe({
        next: (user) => {
          if (user) {
            this.setCurrentUser(user);
          }
        },
        error: () => {
          this.logout();
        }
      })
    },
     15*60* 1000 // Refresh every 15 minutes
    );
  }

  stopTokenRefreshInterval() {
    if (this.refreshTokenIntervalId) {
      clearInterval(this.refreshTokenIntervalId);
    }
  }

  setCurrentUser(user:User){
    user.roles= this.getRolesFromToken(user);
    this.currentUser.set(user);
    this.likesService.getLikeIds();
  }
 
  logout() {
    localStorage.removeItem("filters");    
    this.stopTokenRefreshInterval(); 
    this.currentUser.set(null);
    this.likesService.clearLikeIds();
  }

  private getRolesFromToken(user :User) :string[]{
    const payload = user.token.split(".")[1];
    const decoded=atob(payload);
    const jsonPayload =JSON.parse(decoded);
    return Array.isArray(jsonPayload.role)? jsonPayload.role :[jsonPayload.role];
  }
}
