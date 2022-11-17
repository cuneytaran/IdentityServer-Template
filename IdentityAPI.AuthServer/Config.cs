// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityAPI.AuthServer
{
    public static class Config
    {
        //packet yöneticisinden IdentityServer4 yükle.
        public static IEnumerable<ApiResource> GetApiResources()//ApiResource identityserver tanıyor. Audis lere karşılık geliyor.
        {
            return new List<ApiResource>()//1.API yi tanımlıyoruz.
            {
                new ApiResource("resource_api1"){//ismini biz verdik.apinin tanıtıyoruz ve izin tanımlaması yapıyoruz.
                    Scopes={ "api1.read","api1.write","api1.update" },//tanımlanmış yetkileri atıyoruz (oku,yazma,güncelleme).//INTROSPECTION ENDPOINT için API nin userName= resource_api1 
                    ApiSecrets = new []{new  Secret("secretapi1".Sha256())}//INTROSPECTION ENDPOINT için API nin Password=secretapi1
                },

                new ApiResource("resource_api2")//2.API yi tanımlıyoruz
                {
                       Scopes={ "api2.read","api2.write","api2.update" },//INTROSPECTION ENDPOINT için API nin userName= resource_api2 
                       ApiSecrets = new []{new  Secret("secretapi2".Sha256()) }//INTROSPECTION ENDPOINT için API nin Password=secretapi2
                },

                new ApiResource(IdentityServerConstants.LocalApi.ScopeName)//IdentityServer tanımlıyoruz.
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()//Yetkilerin tanımlandığı yer
        {
            return new List<ApiScope>()
            {
                new ApiScope("api1.read"    ,"API 1 için okuma izni"),//API 1 için okuma izni verdik.
                new ApiScope("api1.write"   ,"API 1 için yazma izni"),
                new ApiScope("api1.update"  ,"API 1 için güncelleme izni"),

                new ApiScope("api2.read"    ,"API 2 için okuma izni"),
                new ApiScope("api2.write"   ,"API 2 için yazma izni"),
                new ApiScope("api2.update"  ,"API 2 için güncelleme izni"),

                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)//identity server için tanımladık. Aslında içine "IdentityServerApi" sabiti yazmış olduk.
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                //identity de ön tanımlı bilgiler bunlar.                ,
                new IdentityResources.Email(),//kullanıcının emailini alabilme
                new IdentityResources.OpenId(), //olmazsa olmazı bu.Token döndüğünde içinde mutlaka kullanıcının id si olmalı buda subjectid olarak geçer.Yani userId si için.
                new IdentityResources.Profile(), //User profil bilgileri. Bunun içinde bir sürü claim tutabilirsiniz.bilgi için adres : https://developer.okta.com/blog/2017/07/25/oidc-primer-part-1
                new IdentityResource(){ Name="CountryAndCity", DisplayName="Country and City",Description="Kullanıcının ülke ve şehir bilgisi", UserClaims= new [] {"country","city"}},//ülke ve şehir bilgisini tutuyoruz ve istediğimiz clientlere vereceğiz.Custom bilgiler ekliyoruz

                new IdentityResource(){ Name="Roles",DisplayName="Roles", Description="Kullanıcı rolleri", UserClaims=new [] { "role"} }//Roller tanımlanıyor ve role isminde claim oluşturduk.Yani token içinde role de gelecek
            };
        }

        public static IEnumerable<Client> GetClients()//Yukarıdaki API lere hangi clientler kullanacak 15 ve 32. satırlardan bahsediyorum
        {
            return new List<Client>(){//Client = identityserver üzerinden geliyor.
                //POSTMAN İLE İSTEK YAPARKEN BURDAKİ BİLGİLERİ EKLİYORUZ alacağı parametreleri https://identityserver4.readthedocs.io/en/latest/endpoints/token.html buradan alabilirsin.
                 new Client()//BU CLIENTLE ÇIKIŞ YAPARSAN SADECE API1 ERİŞEBİLİRSİN
                {
                   ClientId = "Client1",//sitenin ClientId si olacak bunu kullanıcının username gibi düşünebilirsin
                   ClientName="Client 1 app uygulaması",//kullanıcının API den data almak için işimize yarayacak.
                   ClientSecrets=new[] {new Secret("secret".Sha256())},//şifre secret olarak belirledik. ve bunu şifreledik.yani hash ledik.datayı appsettings den almak daha doğru olur.
                   AllowedGrantTypes= GrantTypes.ClientCredentials,//ClientCredentials= bu akışa uygun token verecek. Akış tipini seçtik.bir çok tipler var. Ençok kullanılan bu tipdir.identityserver ile api arasında balantı yapıyor site bazında. ClientCredentialda refresh token olmaz!!!!
                   AllowedScopes= {"api1.read"}//erişim tanımlandığı yer.Hangi API ler bana istek yapabilir. Yani birden fazla API tanımlayabiliriz.Hangi API den nasıl bir yetki ye erişmek isteyeceğimizi belirliyoruz. 
                },
                 new Client()//BU CLIENTLE ÇIKIŞ YAPARSAN API1 ve API2 YE ERİŞEBİLİRSİN
                {
                   ClientId = "Client2",
                   ClientName="Client 2 app uygulaması",
                   ClientSecrets=new[] {new Secret("secret".Sha256())},
                   AllowedGrantTypes= GrantTypes.ClientCredentials,
                   AllowedScopes= {"api1.read" ,"api1.update","api2.write","api2.update"}//erişim tanımlandığı yer
                },
                 new Client()
                 {
                   ClientId = "Client1-Mvc",
                   RequirePkce=false,//secret devre dışı bırakayımmı. Randon challenge ve modifie yapalımmı
                   ClientName="Client 1 app  mvc uygulaması",
                   ClientSecrets=new[] {new Secret("secret".Sha256())},
                   AllowedGrantTypes= GrantTypes.Hybrid,//code id_token kullanıdığımız için hybrid kullanıyoruz.
                   RedirectUris=new  List<string>{ "https://localhost:5006/signin-oidc" },//token alma işlemini gerçekleştiren URL dir.Authorize endpoint den token bu adrese yönlenecek.
                   PostLogoutRedirectUris=new List<string>{ "https://localhost:5006/signout-callback-oidc" },//identity serverden logout işlemi olduğunda bu adrese yönlenecek.
                   AllowedScopes = { //identityResource den yetkileri alıyoruz.OfflineAccess startup tarafında tanımladık. User bilgilerini artık token içinde tanımladık.
                         IdentityServerConstants.StandardScopes.Email,
                         IdentityServerConstants.StandardScopes.OpenId,
                         IdentityServerConstants.StandardScopes.Profile, "api1.read",
                         IdentityServerConstants.StandardScopes.OfflineAccess,"CountryAndCity","Roles"

                     },
                   AccessTokenLifetime=2*60*60,//access token ömrü tanımlıyoruz.Biz 2 saatlik yaptık.Defaultda bunu saniye bazında yapıyoruz.Defaultu 3600 sn yani 1 saat. 2*60*60=saat*dakika*saniye ikinci yol (int) (DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds
                   AllowOfflineAccess=true,//refresh token oluşturma komutu
                   RefreshTokenUsage=TokenUsage.ReUse,//refresh token birdende fazla kulllanılabilsin.Eğer biz OneTimOnly seçersek bir kez refresh token kullanılır. Eğer ReUse seçersek sürekli kendisini yenileme özelliği olmuş olur.
                   RefreshTokenExpiration=TokenExpiration.Absolute,//refresh token da Absolute seçersek kesin bir süre vermiş oluruz. Mesela 5 gün ömrü bitsin. Sliding verirsek default 15 gün. eğer 15 gün içersinde bir refresh token yaparsak tekrar 15 gün daha uzar. 
                   AbsoluteRefreshTokenLifetime=(int) (DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,//Absolute kesin ömür vermiştik. Default da 30 gündür. Ama biz 60 günlük bir refresh token yaptık. 
                   RequireConsent=false//bunu true yaparsan onay sayfasına otomatik yönlendirir.login olduktan sonra ikinci bir onay sayfasına yönlendirir.Yani token içinde hangi bilgiler olsun olmasın diye checkbox tikleyerek seçebiliyorsun.
                   //yukarıda 2 saatlik token ve 60 günlük refresh token hazırladık.
        },
                 new Client()
                 {
                   ClientId = "Client2-Mvc",
                   RequirePkce=false,
                   ClientName="Client 2 app  mvc uygulaması",
                   ClientSecrets=new[] {new Secret("secret".Sha256())},
                   AllowedGrantTypes= GrantTypes.Hybrid,
                   RedirectUris=new  List<string>{ "https://localhost:5011/signin-oidc" },
                   PostLogoutRedirectUris=new List<string>{ "https://localhost:5011/signout-callback-oidc" },
                   AllowedScopes = {
                         IdentityServerConstants.StandardScopes.Email,
                         IdentityServerConstants.StandardScopes.OpenId, 
                         IdentityServerConstants.StandardScopes.Profile, "api1.read","api2.read",
                         IdentityServerConstants.StandardScopes.OfflineAccess,"CountryAndCity","Roles"

                     },//"CountryAndCity","Roles"=token içinde country ve rollerde görünecek
                   AccessTokenLifetime=2*60*60,
                   AllowOfflineAccess=true,
                   RefreshTokenUsage=TokenUsage.ReUse,
                   RefreshTokenExpiration=TokenExpiration.Absolute,
                   AbsoluteRefreshTokenLifetime=(int) (DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,

                   RequireConsent=false
        },
                 new Client()//Angular projesi için
                  {
                     //YAPILAN TÜM DEĞİŞİKLİKLER MEMORY DE TUTULDUĞU İÇİN RESTART ATMAN GEREKİYOR. YOKSA GEÇERLİ OLMAZ DEĞİŞİKLİKLER
                     ClientId="js-client",//bu sitenin id si gibi düşün.
                     RequireClientSecret=false,//ClientSecrets kesinlikle verilmez.o yüzden kapattık. backend uygulamalarda açılır.
                     AllowedGrantTypes=GrantTypes.Code,//Akışı tipi olarak Authorization Code  grant kullanıyoruz.Bu yüzden Code seçiyoruz.
                     ClientName="Js Client (Angular)",//ismini veriyoruz
                     AllowedScopes = { //token içinde neler token içinde gidecek bunu belirliyoruz.
                         IdentityServerConstants.StandardScopes.Email,
                         IdentityServerConstants.StandardScopes.OpenId,
                         IdentityServerConstants.StandardScopes.Profile, "api1.read",
                         IdentityServerConstants.StandardScopes.OfflineAccess,"CountryAndCity","Roles"

                     },
                     //bir angular uygulaması default olarak 4200 porttan ayağa kalkar
                     RedirectUris={"http://localhost:4200/callback"},//login olunca hangi adrese yönlensin
                     AllowedCorsOrigins={"http://localhost:4200"},//Tüm cors izinlerini veriyoruz.
                     PostLogoutRedirectUris={"http://localhost:4200" }//logout olunca gidecek adres
                  },
                 new Client()
                 {
                   ClientId = "Client1-ResourceOwner-Mvc",
                   ClientName="Client 1 app  mvc uygulaması",
                   ClientSecrets=new[] {new Secret("secret".Sha256())},
                   AllowedGrantTypes= GrantTypes.ResourceOwnerPasswordAndClientCredentials,//hem login ol hemde login olmuşsa tokenini kullan
                   AllowedScopes = { 
                         IdentityServerConstants.StandardScopes.Email, 
                         IdentityServerConstants.StandardScopes.OpenId, 
                         IdentityServerConstants.StandardScopes.Profile, "api1.read",
                         IdentityServerConstants.StandardScopes.OfflineAccess,"CountryAndCity","Roles",
                         IdentityServerConstants.LocalApi.ScopeName
                     },
                   AccessTokenLifetime=2*60*60,
                   AllowOfflineAccess=true,
                   RefreshTokenUsage=TokenUsage.ReUse,
                   RefreshTokenExpiration=TokenExpiration.Absolute,
                   AbsoluteRefreshTokenLifetime=(int) (DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,
        },
    };
        }
    }
    //BURDA TANIMLAMALARI YAPTIKTAN SONRA STARTUP A GEÇ 
}
