using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UdemyIdentityServer.Client2.Models;

namespace UdemyIdentityServer.Client2.Services
{
    public interface IApiResourceHttpClient
    {
        Task<HttpClient> GetHttpClient();
        Task<List<string>> SaveUserViewModel(UserSaveViewModel userSaveViewModel);
    }
}