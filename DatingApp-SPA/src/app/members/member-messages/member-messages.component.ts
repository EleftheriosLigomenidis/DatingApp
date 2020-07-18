import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/Message';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
@Input() recipientId:number;
messages:Message[];
newMessage:any= {};
  constructor(private userService: UserService, private alertify: AlertifyService, private authservice:AuthService
   ) { }

  ngOnInit() {
  }


  loadMessages(){
    const currentUserId = +this.authservice.decodedToken.nameid;
    this.userService.getMessageThread(this.authservice.decodedToken.nameid,this.recipientId)
    .pipe(

      tap(messages =>{
        for(let i =0; i<messages.length; i++){
          if(messages[i].isRead === false && messages[i].recipientId ===currentUserId){
            this.userService.markAsRead(currentUserId,messages[i].id);
          }
        }
      })
    )
    .subscribe(messages => {
      this.messages = messages;
    },error => {
      this.alertify.error(error);
    });
  }


  sendMessage(){
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authservice.decodedToken.nameid,this.newMessage)
    .subscribe((message: Message) => {
      
      this.messages.unshift(message);
      this.newMessage.content = "";
    },error => {
      this.alertify.error(error);
    })

  }


}
