import { Component, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { Member, Photo } from '../../../types/member';
import { ActivatedRoute } from '@angular/router';
import { ImageUpload } from "../../../shared/image-upload/image-upload";
import { AccountService } from '../../../core/services/account-service';
import { User } from '../../../types/user';
import { StarButton } from "../../../shared/star-button/star-button";
import { ToastService } from '../../../core/services/toast-service';
import { DeleteButton } from "../../../shared/delete-button/delete-button";

@Component({
  selector: 'app-member-photos',
  imports: [ImageUpload, StarButton, DeleteButton],
  templateUrl: './member-photos.html',
  styleUrl: './member-photos.css',
})
export class MemberPhotos implements OnInit {
  protected memberService = inject(MemberService);
  protected accountService = inject(AccountService);
  private route = inject(ActivatedRoute);
  protected toast=inject(ToastService)
  protected photos = signal<Photo[]>([]);
  protected loading = signal<boolean>(false);

  ngOnInit(): void {
    const memberId = this.route.parent?.snapshot.paramMap.get('id');
    if (memberId) {
      this.memberService.getMemberPhotos(memberId).subscribe({
        next: (data) => this.photos.set(data)
      })
    }
  }

  onUploadImage(file: File) {
    this.loading.set(true);
    this.memberService.uploadPhoto(file).subscribe({
      next: (photo) => {
        this.memberService.editMode.set(false);
        this.photos.set([...this.photos(), photo]);
      },
      error: (error) => { console.log(error); },
      complete: () => this.loading.set(false)
    });
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainImage(photo).subscribe({
      next: () => {
        const currentUser = this.accountService.currentUser();
        if (currentUser) currentUser.imageUrl = photo.url;
        this.accountService.currentUser.set(currentUser as User);
        console.log(this.accountService.currentUser());

        this.memberService.member.update(member => ({ ...member, imageUrl: photo.url }) as Member);
      }
    });
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe({
      next: () => {
        this.photos.update(photos=> photos.filter(p => p.id!==photoId))
      },
      error(err) {
        alert(err);
      },
      })
  }


}
