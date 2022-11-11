import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private authService:AuthService) { }

  status:any;
  ngOnInit(): void {
    this.authService.userManager.getUser().then((user)=>{
      if(user){
        console.log(user);
        this.status='Hoş geldiniz. Login Oldunuz.';
      }else{
        this.status='Login değilsiniz!';
      }
    })
  }

  login():void{
  this.authService.userManager.signinRedirect();//5001 portuna yönleniyor olacağız
  }

  logout():void{
    this.authService.userManager.signoutRedirect();
  }

}
