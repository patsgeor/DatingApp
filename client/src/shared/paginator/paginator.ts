import { Component, computed, input, model, output, signal } from '@angular/core';
import { PaginationMetadata } from '../../types/pagination';
import { min } from 'rxjs';

@Component({
  selector: 'app-paginator',
  imports: [],
  templateUrl: './paginator.html',
  styleUrl: './paginator.css',
})
export class Paginator {
  selectPage=[5,10,20,50];
  pageNumber = model(1);
  totalPages = input(1);
  pageSize = model(10);
  totalCount = input(0);
  lastItemIndex= computed(()=>Math.min(this.totalCount(),
                                      this.pageSize()*this.pageNumber()));
  pageChange=output<{pageNumber:number,pageSize:number}>();


  onPageChange(pageNumber?:number,pageSize?:EventTarget | null){ 
    if(pageNumber) this.pageNumber.set(pageNumber);

    if(pageSize) {
      const size=Number((pageSize as HTMLSelectElement).value)
      this.pageSize.set(size);
    }

    this.pageChange.emit({
      pageNumber: this.pageNumber(),
      pageSize:this.pageSize()
    });
  }



}
