using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using AzureADTenantPart2.Models;
using AzureADTenantPart2.Models.ConData;

namespace AzureADTenantPart2.Controllers
{
    [Route("Account/[action]")]
    public partial class AccountController : Controller
    {
        private ConDataService _conDataService;//creation of private variable for ConData Service
        private readonly Multitenancy _multitenancy;

        public AccountController(ConDataService conDataService, Multitenancy multitenancy)
        {
            _conDataService = conDataService;//injected ConDataService into AccountController
            _multitenancy = multitenancy;

        }
        public IActionResult Login()
        {
            var redirectUrl = Url.Content("~/");

            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Logout()
        {
            var redirectUrl = Url.Content("~/");

            return SignOut(new AuthenticationProperties { RedirectUri = redirectUrl }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpPost]
        public async Task<ApplicationAuthenticationState> CurrentUser()
        {
            if (!string.IsNullOrEmpty(User.Identity.Name))//Ad authentication has taken place
            {
                foreach (var tenant in _multitenancy.Tenants)
                {
                    var temporaryConnectionString = tenant.ConnectionString;
                    _conDataService = new ConDataService(tenant.ConnectionString);

                    bool userIsValid = await _conDataService.UserIsValid(User.Identity.Name);


                    if (userIsValid)
                    {
                        return new ApplicationAuthenticationState
                        {
                            IsAuthenticated = User.Identity.IsAuthenticated,
                            Name = User.Identity.Name,
                            Claims = User.Claims.Select(c => new ApplicationClaim { Type = c.Type, Value = c.Value })
                        };
                    }
                    
                }





            }

            return new ApplicationAuthenticationState
            {
                IsAuthenticated = false,
                Name = null,
                Claims = new List<ApplicationClaim>()
            };




        }



        private async Task<SolutionUser> ValidateCurrentUser()
        {
            SolutionUser userFromAD = await _conDataService.GetSolutionUserByEmail(User.Identity.Name);
            return userFromAD;
        }
    }
}