import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { EditableMember, Member, Photo } from '../../types/member';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private http=inject(HttpClient);
  private baseUrl=environment.apiUrl; //παιρνουμε το apiUrl από το environment
  editMode=signal<boolean>(false);
  member=signal<Member | null>(null);

getMembers() {
    return this.http.get<Member[]>(this.baseUrl+`members` );
}

getMember(id:string) {
    return this.http.get<Member>(this.baseUrl+'members/' + id).pipe(
      tap(member=>
        {this.member.set(member)}
      )
    );
}

getMemberPhotos(id:string) {
  return this.http.get<Photo[]>(this.baseUrl +'members/'+  id  + '/photos')
}

updateMember(member : EditableMember) {
  return this.http.put(this.baseUrl+'members',member);
}

uploadPhoto(file : File){
  const formData=new FormData()//Τα αρχεία δεν στέλνονται ως JSON Το API περιμένει: multipart/form-data, key με όνομα file
  formData.append("file",file);// το όνομα "file" πρεπει να ταιριαζει με αυτο που περιμενει ο server στο PhotoController
  return this.http.post<Photo>(this.baseUrl+'members/add-photo',formData);// δεν στελουμε memberId γιατι το παιρνουμε απο το token
}

setMainImage(photo:Photo){
  return this.http.put(this.baseUrl +'members/set-main-photo/'+photo.id,{});
}

deletePhoto(photoId:number){
  return this.http.delete(this.baseUrl +'members/delete-photo/'+photoId);
}

}
