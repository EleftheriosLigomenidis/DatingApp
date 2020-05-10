import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import {JwtHelperService} from '@auth0/angular-jwt';
import { Router } from '@angular/router';
@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
model: any = {}; // declare model
  constructor(public authService: AuthService, private alertify: AlertifyService,private router:Router) { }

  ngOnInit() {
  }

  login(){
    this.authService.login(this.model).subscribe(next => {
// console.log('Logged in succesfully'); // the procedure is successful

this.alertify.success('successfull login');

    }, error => {
      //console.log(error);
      this.alertify.error(error);
    }, () =>{
      this.router.navigate(['/members']);
    });
  }

  loggedIn(){
    
    return this.authService.loggedIn();
  }

  logout(){
    localStorage.removeItem('token');
    //console.log('logged out');
    this.alertify.message("logged out");
    this.router.navigate(['/home']);
  }
}
