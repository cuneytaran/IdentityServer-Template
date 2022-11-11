using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using UdemyIdentityServer.Client1.Services;

namespace UdemyIdentityServer.Client1
{
    public class Startup
    {
        //nuget package den Microsoft.AspNetCore.Authentication.OpenIdConnect yükle

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

      
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddHttpContextAccessor();//bunu eklersen heryerden contexi ulaşabilirsin.
            services.AddScoped<IApiResourceHttpClient, ApiResourceHttpClient>();//servis içinde interface oluşturduk.Burda onu tanımlıyoruz. Scope her seferinde bir tane örneğini alacak anlamında.

            //MERKEZİ SİSTEME BAĞLANMAK İÇİN services.AddAuthentication KODUNU EKLEYEREK YAPIYORUZ
            //openid ile akış yöntemini belirliyoruz. code id_token yani
            services.AddAuthentication(opts =>
            {
                opts.DefaultScheme = "Cookies";//benim client1 oluşacak Cookie nin ismi Cookies olsun
                opts.DefaultChallengeScheme = "oidc";//OpenId cookie ile haberleşecek.Identity serverden gelen cookie ile haberleşecek
            }).AddCookie("Cookies", opts =>//Cookies isimli şemayı ekle
            {
                opts.AccessDeniedPath = "/Home/AccessDenied";//yetkisi olmayan bir kullanıcı erişmeye çalıştığında bu sayfaya yönlendiriyoruz.
            }).AddOpenIdConnect("oidc", opts =>//oidc isimli OpenId yi şemaya ekler
            {
                opts.SignInScheme = "Cookies";//bizim cookie oluşturuyoruz
                opts.Authority = "https://localhost:5001";//Token dağıtan yerin adresi. IdentitiyServer yani
                opts.ClientId = "Client1-Mvc";//ClientId si
                opts.ClientSecret = "secret";//ClientId şifresi
                opts.ResponseType = "code id_token";//code:Access token, id_token=doğru yerdenmi gelmiş belirlemek için.
                opts.GetClaimsFromUserInfoEndpoint = true;//cookie içine user bilgilerinide ekler.
                opts.SaveTokens = true;//Başarılı bir Authorize işleminden sonra Access ve Refresh token kaydediliyor.
                opts.Scope.Add("api1.read");//identity serverden api1 için okuma izni ver .identityserver config içinde AllowedScopes içinde bu proje için izin vermiştik. api1.read. identity serverdeki izin ne isi bundada aynı izini istemelisin yoksa hata verir.
                opts.Scope.Add("offline_access");//refresh token alıyoruz
                opts.Scope.Add("CountryAndCity");//config.cs dosyasındaki country bilgisini çekiyoruz
                opts.Scope.Add("Roles");//rolleride ekle diyoruz
                opts.Scope.Add("email");//emaili ekliyoruz
                opts.ClaimActions.MapUniqueJsonKey("country", "country");//token içindeki country ye mapledik.
                opts.ClaimActions.MapUniqueJsonKey("city", "city");//token içine city ekliyoruz
                opts.ClaimActions.MapUniqueJsonKey("role", "role");//token içindeki role mapladik

                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    RoleClaimType = "role",// token bazlı doğrulama var ve rol olarak yukardaki role ü seç diyoruz.
                    NameClaimType="name"//herhangi bir clientin içinde User.Identity.name den username ulaşabilmek için
                };
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();//önce kimlik doğrulama yapılır
            app.UseAuthorization();//sonra yetkileri erişilir

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}