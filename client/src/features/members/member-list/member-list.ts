import { Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { Member } from '../../../types/member';
import { MemberCard } from "../member-card/member-card";
import { MemberParams, PaginatedResult } from '../../../types/pagination';
import { Paginator } from "../../../shared/paginator/paginator";
import { FilterModal } from "../filter-modal/filter-modal";

@Component({
  selector: 'app-member-list',
  imports: [MemberCard, Paginator, FilterModal],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css',
})
export class MemberList implements OnInit {
  @ViewChild('filterModal') modal: FilterModal;
  private memberService = inject(MemberService);
  protected paginatedMembers = signal<PaginatedResult<Member> | null>(null);
  protected memberParams = new MemberParams();
  protected updateParams = new MemberParams();

  constructor(){
    const filters= localStorage.getItem('filters');
    if(filters) {
      this.memberParams=JSON.parse(filters);
      this.updateParams=JSON.parse(filters);
    }
  }



  ngOnInit(): void {
    this.loadPage();
  }

  loadPage() {
    this.memberService.getMembers(this.memberParams).subscribe({
      next: (data) => this.paginatedMembers.set(data)
    });
  }

  onPagechange(event: { pageNumber: number; pageSize: number }) {
    this.memberParams.pageNumber = event.pageNumber;
    this.memberParams.pageSize = event.pageSize;
    this.loadPage();
  }

  openModal(){
    this.modal.open();
  }

  onClose(){
    this.modal.close();
  }

  onFilterChange(data :MemberParams){
    this.memberParams={...data};
    this.updateParams={...data};
    this.loadPage();
  }

  get displayFilter(){
    const defaultParams=new MemberParams();
    let filter: string[] =[];

    if(this.updateParams.gender){
      filter.push(this.updateParams.gender + 's')
    }
    else{
      filter.push('Males & Females')
    }

    if(this.updateParams.minAge !== defaultParams.minAge ||
      this.updateParams.maxAge !==defaultParams.maxAge
    ){
      filter.push(`Age: ${this.updateParams.minAge}-${this.updateParams.maxAge}`)
    }

    filter.push(this.updateParams.orderBy ==='lastActive'? 'Recently active':'Newest member');
    
    return filter.length>0 ? 'Selected : '+ filter.join('  | '): 'All members';
  }

  resetParams(){
    this.memberParams= new MemberParams();
    this.updateParams=new MemberParams();
    this.loadPage();
  }
}
