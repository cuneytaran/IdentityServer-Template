using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UdemyIdentityServer.Client1.Models;

namespace UdemyIdentityServer.Client1.Services
{
    public class ApiResourceHttpClient : IApiResourceHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        private HttpClient _client;

        public ApiResourceHttpClient(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _client = new HttpClient();
        }

        public async Task<HttpClient> GetHttpClient()
        {
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            _client.SetBearerToken(accessToken);

            return _client;
        }

        public async Task<List<string>> SaveUserViewModel(UserSaveViewModel userSaveViewModel)
        {
            var disco = await _client.GetDiscoveryDocumentAsync(_configuration["AuthServerUrl"]);

            if (disco.IsError)
            {
                //loglama yap
            }

            var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest();//client token alma işlemi.

            clientCredentialsTokenRequest.ClientId = _configuration["ClientResourceOwner:ClientId"];//appsettingsden ClientId alıyoruz configurationdan
            clientCredentialsTokenRequest.ClientSecret = _configuration["ClientResourceOwner:ClientSecret"];//appsettingsden ClientSecret alıyoruyoruz
            clientCredentialsTokenRequest.Address = disco.TokenEndpoint;//endpoint adresi veriyoruz

            var token = await _client.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);//bilgileri yolluyoruz ve tokeni alıyoruz

            if (token.IsError)
            {
                //loglama yap
            }

            var stringConent = new StringContent(JsonConvert.SerializeObject(userSaveViewModel), Encoding.UTF8, "application/json");//json formatında bilgileri ekliyoruz.

            _client.SetBearerToken(token.AccessToken);//bearer e token ekliyoruz

            var response = await _client.PostAsync("https://localhost:5001/api/user/signup", stringConent);//tokenla birlikte bilgileri gönderiyoruz

            if (!response.IsSuccessStatusCode)
            {
                var errorList = JsonConvert.DeserializeObject<List<string>>(await response.Content.ReadAsStringAsync());

                return errorList;
            }

            return null;
        }
    }
}