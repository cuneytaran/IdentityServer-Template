using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using UdemyIdentityServer.Client1.Models;
using UdemyIdentityServer.Client1.Services;

namespace UdemyIdentityServer.Client1.Controllers
{
    //nuget package den IdentityModel kur
    [Authorize] //Authorize buraya koyarsan controller içindeki hepsine kapsar
    public class ProductsController : Controller
    {
        private readonly IConfiguration _configuration;//appsetting deki datayı okumak için.
        private readonly IApiResourceHttpClient _apiResourceHttpClient;

        public ProductsController(IConfiguration configuration, IApiResourceHttpClient apiResourceHttpClient)
        {
            _configuration = configuration;
            _apiResourceHttpClient = apiResourceHttpClient;
        }


        public async Task<IActionResult> Index()
        {
            List<Product> products = new List<Product>();
            //clienti oluşturmak için yani context oluşturmak için heryerde ulaşmak için ApiResourceHttpClient adında bir servis oluşturudum. her yerde buraya ulaşıyorum.
                                                                             //startup configure servis içinde
                                                                             //services.AddHttpContextAccessor(); bunu eklersen heryerden contexi ulaşabilirsin.
                                                                             //services.AddScoped<IApiResourceHttpClient, ApiResourceHttpClient>();
                                                                             //tanımlamayı unutma!!!
            HttpClient client = await _apiResourceHttpClient.GetHttpClient();////Tokeni artık otomatik olarak ekleyecek

            //https://localhost:5006

            var response = await client.GetAsync("https://localhost:5016/api/products/getproducts");

            //Aşağıdaki işlemleri yapabilmek için. api1.update, api1.delete vs.. tanımlamaları yapman gerekiyor. config.cs den
            ////post işlemi için
            //var PostResponse = await client.PostAsync("https://localhost:5016/api/products/getproducts");
            ////update işlemi için
            //var UpdateResponse = await client.PutAsync("https://localhost:5016/api/products/getproducts");
            ////delete işlemi için
            //var DeleteResponse = await client.DeleteAsync("https://localhost:5016/api/products/getproducts");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                products = JsonConvert.DeserializeObject<List<Product>>(content);
            }
            else
            {
                //loglama yap
            }

            return View(products);
        }
    }
}