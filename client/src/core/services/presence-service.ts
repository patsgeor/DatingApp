import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { ToastService } from './toast-service';
import { User } from '../../types/user';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { Message } from '../../types/message';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  private baseUrl =environment.hubUrl;
  private toast =inject(ToastService);
  public hubConnection? : HubConnection;
  onlineUsers = signal<string[]>([]);

  createHubConnection(user:User){
    this.hubConnection = new HubConnectionBuilder()
                              .withUrl(this.baseUrl+'presence/', {accessTokenFactory: ()=> user.token} )
                              .withAutomaticReconnect()
                              .build();
    
    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection?.on("UserOnline", userId =>{
        this.onlineUsers.update(users =>[...users,userId]);
      }      
    )

    this.hubConnection?.on("GetOnlineUsersId",userIds => this.onlineUsers.set(userIds));

    this.hubConnection?.on("UserOffline", userId =>{
        this.onlineUsers.update(u => u.filter(id => id !== userId))
      }
    )

    this.hubConnection.on('NewMessageReceived', (message:Message) => {
      this.toast.info(message.senderDisplayName + 'has sent you a new message', 
                    10000,  message.senderImageUrl,  `/members/${message.senderId}/messages`)
      }
    )
  }

  stopHubConnection(){
   if( this.hubConnection?.state === HubConnectionState.Connected){
    this.hubConnection.stop().catch(error => console.log(error));
   }
  }
  
}
