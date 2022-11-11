// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityAPI.AuthServer.Data;
using IdentityAPI.AuthServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityAPI.AuthServer.Services;

namespace IdentityAPI.AuthServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));//ApplicationDbContext=ApplicationUser entitytisine temsil ediyor. buradaki UseSqlServer,UseSqlLite,MySql vs... istediğini tanımlayabilirsin.
           
            services.AddIdentity<ApplicationUser, IdentityRole>()//identity serverve identity API sini eklemiş.IdentityRole identity API den gelen role dür. NASILKİ ApplicationUser MODELDEKİ ApplicationUser dan miras alıyorsa sende rolleri genişletmek istiyorsan sende modele bir tane oluşturup miras aldırtarak rolleri genişletebilirsin.
                .AddEntityFrameworkStores<ApplicationDbContext>()//Entity Framework Core kullanacağımızı belirttik
                .AddDefaultTokenProviders();//Identitiy üyelik sistemin şifre sıfırlamak için kullanılan token provider dir. 
            
            var builder = services.AddIdentityServer(options => // bir hata aldığınızda , loglama yaptığında vs... loglamak için imkan sunuyor
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                //in Memory yapısı oluşturuldu. Yani ramda tutulacak.bilgiler.
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiScopes(Config.GetApiScopes())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>()//identity ile konuşması için AddAspNetIdentity eklenmiş ve generic olarak ApplicationUser tanımlanmış.
                .AddResourceOwnerValidator<IdentityResourceOwnerPasswordValidator>();//ÖNEMLİ!!!=AddAspNetIdentity<ApplicationUser> altında olması gerekiyor.Altına ekliyoruzki. usarname mi email olarak değiştirmek için eziyoruz.
            
            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();//tempkey.jwk dosyasını oluşturuyor. yani geçici bilgileri bir yerde tutuyor.

            services.AddAuthentication()//google ile entegre yapılma işlemi eklendi.burdaki eklenti identity login ekranı için geçerli
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google//google bu redirect url ni google daki aplication kısmına veriyorsun.yani login olunca hangi sayfaya gideceğini belirtiyoruz.
                    options.ClientId = "copy client ID from Google here";//googledan alacağın clientId yi buraya yapıştırıyorsun
                    options.ClientSecret = "copy client secret from Google here";//client secreti buraya yapıştırıyorsun                   
                });

            services.AddLocalApiAuthentication();//identity server artık API gibi davranacak, ve dışara endpoint açacak ama mutlaka token ile erişebilsin.Controllerde [Authorize(LocalApi.PolicyName)] deki PolicyName burdan alıyor.
            services.AddControllersWithViews();//normal controller tanımlanmış
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}