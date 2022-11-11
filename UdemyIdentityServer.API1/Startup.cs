using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;

namespace UdemyIdentityServer.API1
{
    public class Startup
    {
        //nuget packet ten Microsoft.AspNetCore.Authentication.JwtBearer yükle

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

      
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            //JWT token kurulumu yapıyoruz
            //burası bayi ve normal üyelik sistemini bir birine ayırmak için şema yapısı kullanılıyor.
            //başka yazım şekli 
            // services.AddAuthentication("normalUye").AddJwtBearer("normalUye", opts =>
            // services.AddAuthentication("bayiUye").AddJwtBearer("bayiUye", opts =>
            //uygulamamdaki şema ismi ile json web token şemadaki isimle aynı olursa birbirine bağlanmış olur.
            //defaultta gelen yazım şekli
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://localhost:5001";//jwt token yani acces token yayınlayan yani yetki kim adresi.yani (identityserver adresi). Buruya bir token geldiğinde bu API gidecek bu adresten public key alacak. idenditity deki private key ile doğrulayacak. kilit anahtar ilişki sayesinde.
                options.Audience = "resource_api1";//Benden data alacak kişide mutlaka ismi olmalı. yani jwt içindeki aut daki bilgi. Bana bir token gelidiğinde mutlaka aut alanındaki isimle aynı olmalı.resource_api1 bunu identiyserver projesinin içinde config.cs dosyasının içinde tanımladık.
            });

            //kimliği doğrulanmış bir kullanıcının yetkilendirmesi yapılıyor
            services.AddAuthorization(opts =>
            {
                //policy=koşul, şartname
                opts.AddPolicy("ReadProduct", policy =>
                {
                    //token içindeki scope içindeki api1 olanı için sadece okuma iznini ver
                    policy.RequireClaim("scope", "api1.read"); //ilgili action methot mutlaka scope dunda api1.read bir data bekleyecek
                });

                opts.AddPolicy("UpdateOrCreate", policy =>
                {
                    //token içindeki scope içindeki api1 olanı için update ve create iznini ver
                    policy.RequireClaim("scope", new[] { "api1.update", "api1.create" });//ilgili action methot mutlaka scope dunda ya api1.update yada api1.create bir data bekleyecek
                });
            });

            services.AddControllers();
        }

    
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            //sıralama önemli ilk UseAuthentication sonra UseAuthorization eklenmeli
            app.UseAuthentication();//middleware olarak bu eklenmeli. jwt token için gerekli.Kimlik doğrulama.Örn:Doğru site mi.
            app.UseAuthorization();//Yetkilendirme.Örn: Şu metota erişebilecekmi.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}