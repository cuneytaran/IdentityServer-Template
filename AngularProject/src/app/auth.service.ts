import { Injectable } from '@angular/core';
import * as oidc from 'oidc-client';//bu şekilde yaparsan oidc kütüphanesi içindeki tüm özellikleri ulaşabiliyorum anlamına geliyor.
@Injectable({
  providedIn: 'root'
})
export class AuthService {

  config={
    authority:'https://localhost:5001',//token dağıtan yeri belirtiyoruz
    client_id:'js-client',//identity nin tanıyacağı clientid si
    redirect_uri:'http://localhost:4200/callback',//başarılı bir şekilde giriş yaparsa hangi adrese yönlenecek
    response_type:'code',//Authorization akış tipini seçiyoruz. identityserverde bunu tanımladık.
    scope:'openid profile email api1.read',//izinlerimizi istiyoruz.nelere ihtiyacımız var onları identityserverden istiyoruz. ama identitiy serverde de aynı bilgiler olmalı. identity serverde config.cs dosyasının içinde bu bilgileri bulabilirsin.
    post_logout_redirect_uri:'http://localhost:4200'//çıkış yapıldığında
  }

  userManager;

  constructor() {
    this.userManager=new oidc.UserManager(this.config);//bu UserManager üzerinden login olan kulalnıcı bilgilerini alabilirim.
  }
}
