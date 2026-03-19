import { Component, inject, OnInit, signal } from '@angular/core';
import { MessageService } from '../../core/services/message-service';
import { Message } from '../../types/message';
import { PaginatedResult } from '../../types/pagination';
import { Paginator } from "../../shared/paginator/paginator";
import { DatePipe } from '@angular/common';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-messages',
  imports: [Paginator, DatePipe, RouterLink],
  templateUrl: './messages.html',
  styleUrl: './messages.css',
})
export class Messages implements OnInit {
  private messageService = inject(MessageService);
  protected container = "Inbox";
  protected fetchContainer = "Inbox";
  protected pageSize = 15;
  protected pageNumber = 1;
  protected paginatedMessages = signal<PaginatedResult<Message> | null>(null);

  tabs = [{ label: "Inbox", value: "Inbox" },
  { label: "Outbox", value: "Outbox" }
  ]


  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.messageService.getMessages(this.container, this.pageNumber, this.pageSize)
      .subscribe({
        next: data => {
          this.paginatedMessages.set(data)
          this.fetchContainer = this.container;
        }
      });
  }

  get IsInbox() {
    return this.fetchContainer === 'Inbox';
  }

  setContainer(container: string) {
    this.container = container;
    this.loadMessages();
  }


  deleteMessage(event: Event, id: string) {
    event.stopPropagation();
    this.messageService.deleteMessage(id).subscribe({
      next: () => {
        const current = this.paginatedMessages();
        if (current?.items) {
          this.paginatedMessages.update(prev => {
            if (!prev) return null;

            const newItems = prev.items.filter(x => x.id != id) || [];

            return {
              items: newItems,
              metadata: prev.metadata
            }
          })
        }
      }
    })
  }

  onPagechange(event: { pageNumber: number; pageSize: number }) {
    this.pageNumber = event.pageNumber;
    this.pageSize = event.pageSize;
    this.loadMessages();
  }


}
