using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace UdemyIdentityServer.Client1.Controllers
{
    [Authorize]//sadec üye olanlar girebilecek
    public class UserController : Controller
    {
        //http://5001.../user olarak girdiğinde seni merkezi logine yönlendirir ve tekrar geri döner.
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }


        //logout yani çıkış butonu işlemi
        public async Task LogOut()
        {
            await HttpContext.SignOutAsync("Cookies");//browserden hangi isimli cookie sileceğiz. bunu startupdan bakabiliriz.
            await HttpContext.SignOutAsync("oidc");//identity serverden yani OpenId den çıkış yapıp tekrar geri gelecek.string ismi ise startupdan ve identity serverda var.
        }


        //refresh token alma işlemi
        public async Task<IActionResult> GetRefreshToken()
        {
            //iki şekildede username alınabilir.
            var username=User.Claims.First(x=>x.Type=="name").Value;
            var username2 = User.Identity.Name;//bu şekilde username almak istiyorsan mutlaka startup opts.TokenValidationParameters içinde NameClaimType="name" olarak tanımlamılısın.

            HttpClient httpClient = new HttpClient();
            var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");

            if (disco.IsError)
            {
                //loglama yap
            }

            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);//refresh token alıyoruz.RefreshToken yerine AccessToken yaparsak AccessToken almış oluruz.

            //aldığımız refresh tokeni modele dolduruyoruz
            RefreshTokenRequest refreshTokenRequest = new RefreshTokenRequest();
            refreshTokenRequest.ClientId = _configuration["ClientResourceOwner:ClientId"];//clientid yi alıyoruz.appsettings.json dan secret bilgisini çekiyoruz
            refreshTokenRequest.ClientSecret = _configuration["ClientResourceOwner:ClientSecret"];//secret key. configuration dan okuyoruz. yani appsettings.json dan.Client1Mvc:ClientSecret=Client1Mvc den ClientSecret e ulaşıyoruz
            refreshTokenRequest.RefreshToken = refreshToken;//yukarıdan çektiğimiz refresh token
            refreshTokenRequest.Address = disco.TokenEndpoint;//yukarıda disco içinde tüm endpointleri çektik identity den. biz TokenEndpoint i alıyoruz.

            var token = await httpClient.RequestRefreshTokenAsync(refreshTokenRequest);//refresh tokenle birlikte tüm tokenleri çekiyoruz.(refresh, Access vs...)

            if (token.IsError)
            {
                //yönlendirme yap
            }
            var tokens = new List<AuthenticationToken>()//yeni gelen tüm tokenleri ayıklıyoruz.
            {
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.IdToken,Value= token.IdentityToken},//IdToken yani identityToken alıyoruz
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.AccessToken,Value= token.AccessToken},//AccesToken alıyoruz
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.RefreshToken,Value= token.RefreshToken},//Refresh Token alıyoruz
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.ExpiresIn,Value= DateTime.UtcNow.AddSeconds(token.ExpiresIn).ToString("o", CultureInfo.InvariantCulture)}//tokenin ömrünü çekiyoruz.CultureInfo.InvariantCulture=Tüm ülkelerde aynı saati göster.Saatte değişiklik yapma
            };

            var authenticationResult = await HttpContext.AuthenticateAsync();//Authenticate verilerini alıyoruz

            var properties = authenticationResult.Properties;//Authenticate nin propertilerinide alıyoruz.index sayfasında bunu göstermiştik aslında ordan bakabirsin hangi bilgiler geliyor diye.

            properties.StoreTokens(tokens);//tokenlar artık set edilmiş oldu. Yani bu tokenler geçerli olmuş oldu.Token ve refresh token değişikliğini görmezden geliyoruz.

            await HttpContext.SignInAsync("Cookies", authenticationResult.Principal, properties);//Principal=kimlik Bir kimlik kartı gibi düşün, burdaki ad soyad doğumtarihi vs... kullanıcının bilgilerini gösteren bir kimlik ver.Yani SignIn olduğunda yei cookilerle ilgili bilgileri güncelledik.

            return RedirectToAction("Index");
        }



        //admin rollü kullanıcılar bu sayfaya açabilir
        [Authorize(Roles = "admin")]
        public IActionResult AdminAction() 
        {
            return View();
        }

        //admin ve customer rollü  kullanıcılar bu sayfaya açabilir
        [Authorize(Roles = "admin,customer")]
        public IActionResult CustomerAction()
        {
            return View();
        }
    }
}