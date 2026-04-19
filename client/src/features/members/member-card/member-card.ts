import { Component, computed, inject, input } from '@angular/core';
import { Member } from '../../../types/member';
import { RouterLink } from "@angular/router";
import { AgePipe } from "../../../core/pipes/age-pipe";
import { LikesService } from '../../../core/services/likes-service';
import { PresenceService } from '../../../core/services/presence-service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink, AgePipe],
  templateUrl: './member-card.html',
  styleUrl: './member-card.css',
})
export class MemberCard {
  likeServise=inject(LikesService);
  presenceService =inject(PresenceService);
  member = input.required<Member>();
  hasLiked=computed(()=> this.likeServise.likeIds().includes(this.member().id));
  isOnline=computed(()=> this.presenceService.onlineUsers().includes(this.member().id));

  toggleLike(event: Event){
    event.stopPropagation();

    this.likeServise.toggleLike(this.member().id).subscribe({
      next: () =>{
        if(this.likeServise.likeIds().includes(this.member().id)){
          this.likeServise.likeIds.update(ids => ids.filter(x => x!==this.member().id))
        }else{
          this.likeServise.likeIds.update(ids => [...ids,this.member().id])
        }
      }
    })
  }


}
