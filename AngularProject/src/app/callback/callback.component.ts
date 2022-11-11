import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as oidc from 'oidc-client';//bu şekilde yaparsan oidc kütüphanesi içindeki tüm özellikleri ulaşabiliyorum anlamına geliyor.
@Component({
  selector: 'app-callback',
  templateUrl: './callback.component.html',
  styleUrls: ['./callback.component.css']
})
export class CallbackComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit(): void {
    new oidc.UserManager({ response_mode: 'query' })//response_mode=bir istek yapıldığında authorization end point, gelen dataları query içinden oku.
      .signinRedirectCallback()
      .then(() => {
        this.router.navigateByUrl("/");
      });
  }



}
