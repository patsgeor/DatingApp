import { Component, inject, OnInit, signal } from '@angular/core';
import { LikesService } from '../../core/services/likes-service';
import { Member } from '../../types/member';
import { MemberCard } from "../members/member-card/member-card";
import { LikesParams, PaginatedResult } from '../../types/pagination';
import { Paginator } from "../../shared/paginator/paginator";

@Component({
  selector: 'app-lists',
  imports: [MemberCard, Paginator],
  templateUrl: './lists.html',
  styleUrl: './lists.css',
})
export class Lists implements OnInit {
  private likesService = inject(LikesService);
  protected likesParams= new LikesParams();
  protected paginatedMembers = signal<PaginatedResult<Member> | null>(null);


  tabs = [
    { label: 'Liked', value: 'liked' },
    { label: 'Liked me', value: 'likedBy' },
    { label: 'Mutual', value: 'mutual' },
  ]

  ngOnInit(): void {
    this.loadLikes();
  }

  setPredicate(predicate: string) {
    if (this.likesParams.predicate !== predicate) {
      this.likesParams.predicate = predicate;
      this.likesParams.pageNumber=1;
      this.loadLikes();
    }
  }

  loadLikes() {
    this.likesService.getMemberList(this.likesParams).subscribe({
      next: data => this.paginatedMembers.set(data)
    });
  }

   onPagechange(event: { pageNumber: number; pageSize: number }) {
    this.likesParams.pageNumber = event.pageNumber;
    this.likesParams.pageSize = event.pageSize;
    this.loadLikes();
  }


}
