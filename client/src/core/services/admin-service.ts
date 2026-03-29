import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { User } from '../../types/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private http=inject(HttpClient);
  private baseUrl=environment.apiUrl;


  getUsersWithRoles(){
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  editRoles(userId:string,roles:string[]){
    return this.http.post<string[]>(this.baseUrl+"admin/edit-roles/"+userId+"?roles="+roles,{});
  }
  
}
