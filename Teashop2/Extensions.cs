using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Teashop2.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsInPolicy(this ClaimsPrincipal user, string policyName, IServiceProvider serviceProvider)
        {
            IAuthorizationService authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
            var result = authorizationService.AuthorizeAsync(user, policyName).Result;
            
            return result.Succeeded;
        }
    }
}
