import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/Message';
import { Pagination, PaginationResult } from '../_models/Pagination';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
messages:Message[];
pagination:Pagination;
messageContainer = 'Unread';

  constructor(private authService:AuthService,
    private alertify : AlertifyService, private userService: UserService,private route:ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.messages = data['messages'].result;
      this.pagination = data['messages'].pagination;
    });
  }


  loadMessages(){
    this.userService.getMessages(this.authService.decodedToken.nameid,this.pagination.currentPage,this.pagination.itemsPerPage,this.messageContainer)
    .subscribe((res:PaginationResult<Message[]>)=>{
this.messages = res.result;
this.pagination = res.pagination;
    },error =>{this.alertify.error(error);}
    )
  }

  deleteMessage(id:number){
    this.alertify.confirm('Are you sury?',() =>{
      this.userService.deleteMessage(id,this.authService.decodedToken.nameid).subscribe(() =>{
        // remove the message from the browser view  1 means that i want to delete 1 of these
        this.messages.splice(this.messages.findIndex(m => m.id ==id),1);
        this.alertify.success("message has been deleted");
      },error =>{
        this.alertify.error("failed to delete message");
      })
    })
  }

  pageChanged(event: any): void{
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }




  
}
