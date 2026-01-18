import { Component, HostListener, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { EditableMember, Member } from '../../../types/member';
import { DatePipe } from '@angular/common';
import { MemberService } from '../../../core/services/member-service';
import { ToastService } from '../../../core/services/toast-service';
import { FormsModule, NgForm } from '@angular/forms';
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-member-profile',
  imports: [DatePipe, FormsModule],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css',
})
export class MemberProfile implements OnInit, OnDestroy {
  @ViewChild('editForm') editForm?: NgForm; //το χρησιμοποιουμε για να εχουμε προσβαση στη φορμα στο template και να μπορουμε να κανουμε πχ reset 
  @HostListener('window:beforeunload', ['$event']) notify($event:BeforeUnloadEvent){
    if(this.editForm?.dirty){
      $event.preventDefault();
    }
  }
  private toast = inject(ToastService)
  protected memberService = inject(MemberService)
  private accountService = inject(AccountService);
  
  // δημιουργουμε ενα αντικειμενο editableMember που θα κραταει τις τιμες απο τα input πεδια της φορμας
  editableMember: EditableMember = {
    displayName: '',
    city: '',
    description: '',
    country: '',
  }

  ngOnInit(): void {    
    
    // αρχικοποιουμε το editableMember με τις τιμες απο το member οταν φορτωνει η σελιδα
    this.editableMember = {
      displayName: this.memberService.member()?.displayName || '',
      city: this.memberService.member()?.city || '',
      description: this.memberService.member()?.description || '',
      country: this.memberService.member()?.country || '',
    }
  }

  updateProfile() {
    if (!this.memberService.member) return;
    const updatedMember = { ...this.memberService.member(), ...this.editableMember }// συγχωνευουμε τα δυο αντικειμενα, για τα πεδια που υπαρχουν και στα δυο παιρνουμε τις τιμες απο το editableMember το τελευταιο  αντικειμενο
    this.memberService.updateMember(this.editableMember)
      .subscribe({
        next: ()=>{
          this.memberService.member.set(updatedMember as Member);
          let currentUser=this.accountService.currentUser();
          if(currentUser.displayName!=updatedMember.displayName){
            currentUser.displayName=updatedMember.displayName
            this.accountService.currentUser.set(currentUser);
          }
          this.toast.success('Profile updated successfully!');
          this.memberService.editMode.set(false);
          this.editForm?.reset(updatedMember);
        }
      })
    
  }

  // για να κλεισει το edit mode οταν φυγει απο τη σελιδα
  ngOnDestroy(): void {
    if (this.memberService.editMode()) {
      this.memberService.editMode.set(false)
    }
  }
}
