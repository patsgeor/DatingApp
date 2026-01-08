import { ResolveFn, Router } from '@angular/router';
import { MemberService } from '../../core/services/member-service';
import { inject } from '@angular/core';
import { EMPTY } from 'rxjs';
import { Member } from '../../types/member';

export const memberResolver: ResolveFn<Member> = (route, state) => {
  const memberService=inject(MemberService);
  const router=inject(Router);
  const id=route.paramMap.get('id');//route.params['id'];
  const member=memberService.getMember(id);

  if(!member) {
    router.navigateByUrl('/not-fount')//ανακατευθύνουμε σε σελίδα 404
    return EMPTY;// επιστρέφουμε ένα κενό observable αντικείμενο
  } 
  return member;
};
