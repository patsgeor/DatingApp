import { Component, inject } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { Photo } from '../../../types/member';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-member-photos',
  imports: [AsyncPipe],
  templateUrl: './member-photos.html',
  styleUrl: './member-photos.css',
})
export class MemberPhotos {
  private memberService=inject(MemberService);
  private route =inject(ActivatedRoute);

// τρόπος με Observable
 protected photos$? :Observable<Photo[]>;

 constructor(){// καλυτερος τρόπος για να πάρουμε το id από το route ειναι μέσω onInit
  const memberId=this.route.parent?.snapshot.paramMap.get('id');
  if(memberId){
    this.photos$=this.memberService.getMemberPhotos(memberId);
  }
 }

 // Χρησιμοποιούμε getter για να επιστρέψουμε τα ψεύτικα δεδομένα
 get photoMocks() {
  return Array.from({ length: 20 }, (_, i) => ({
    id:i+1,
    url: 'user.png' // Η προεπιλεγμένη εικόνα
  }));
}
}
