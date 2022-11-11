using IdentityAPI.AuthServer.Models;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAPI.AuthServer.Services
{
    public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var existUser = await _userManager.FindByEmailAsync(context.UserName);//UserName=login sayfasındaki formdaki username ama biz oradan email gireceğiz.email üzerinde bir kullanıcı varmı kontrolü

            if (existUser == null) return;//böyle bir user yoksa hiç devam etme

            var passwordCheck = await _userManager.CheckPasswordAsync(existUser, context.Password);//user ve password kontrolü yapılıyor

            if (passwordCheck == false) return;

            context.Result = new GrantValidationResult(existUser.Id.ToString(), OidcConstants.AuthenticationMethods.Password);//user id ve akış tipini belirtiyoruz

            //STARTUP TARAFINDA EKLE
        }
    }
}