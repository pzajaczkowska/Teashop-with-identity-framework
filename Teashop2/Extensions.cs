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

        public static IAsyncEnumerable<TEntity> AsAsyncEnumerable<TEntity>(this Microsoft.EntityFrameworkCore.DbSet<TEntity> obj) where TEntity : class
        {
            return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsAsyncEnumerable(obj);
        }
        public static IQueryable<TEntity> Where<TEntity>(this Microsoft.EntityFrameworkCore.DbSet<TEntity> obj, System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return System.Linq.Queryable.Where(obj, predicate);
        }
    }
}
