import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
registerMode: boolean;
  constructor(private http: HttpClient) { }
values: any;
  ngOnInit() {
    this.getValues();
  }

  registerToggle(){
    this.registerMode = true;
  }

  getValues(){
    this.http.get('http://localhost:50260/api/values').subscribe(response => {  this.values = response;

    }, error => {
      console.log(error);
    });
  }

  cancelRegisterMode(register: boolean){
    this.registerMode = register;

  }
}
