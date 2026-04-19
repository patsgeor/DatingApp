import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Message } from '../../types/message';
import { PaginatedResult } from '../../types/pagination';
import { AccountService } from './account-service';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { ToastService } from './toast-service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  private hubUrl = environment.hubUrl;
  public hubConnection?: HubConnection;
  private accountService = inject(AccountService);
  private toast = inject(ToastService);
  public messageThread = signal<Message[]>([]);

  getMessages(container: string, pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
    params = params.append('container', container);
    return this.http.get<PaginatedResult<Message>>(this.baseUrl + 'messages/', { params });
  }

  createHubConnection(otherUserId: string) {
    var currentUser = this.accountService.currentUser();
    if (!currentUser) return;
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?userId=' + otherUserId, { accessTokenFactory: () => currentUser.token })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('GetMessageThread', (messages: Message[]) => {
      this.messageThread.set(messages.map(m => ({ ...m, currentUserSender: m.senderId === currentUser.id }))
      )
    });

    this.hubConnection.on('NewMessage', (message:Message) => {
      message.currentUserSender= message.senderId ===currentUser.id;
      this.messageThread.update(m => [...m,message]);
    })

    
  }

  sendMessage(recipientId: string, content: string) {
   return this.hubConnection.invoke('SendMessage',{recipientId:recipientId,content:content});
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch(error => console.log(error));
    }
  }

  getMessageThread(memberId : string){
    return this.http.get<Message[]>(this.baseUrl+'messages/thread/'+memberId);
  }

  // sendMessage(recipientId :string, content :string){
  //   return this.http.post<Message>(this.baseUrl+ 'messages',{recipientId:recipientId,content:content})
  // }
 

  deleteMessage(id: string) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
