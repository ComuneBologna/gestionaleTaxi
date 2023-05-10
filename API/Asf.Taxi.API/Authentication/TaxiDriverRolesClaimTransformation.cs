using Asf.Taxi.DAL;
using Asf.Taxi.DAL.Entities;
using SmartTech.Common.API.Security;
using SmartTech.Common.ApplicationUsers.Entities;
using SmartTech.Common.Services;
using SmartTech.Infrastructure.Caching;
using System.Security.Claims;

namespace Asf.Taxi.API.Authentication
{
    public class TaxiDriverRolesClaimTransformation :
        RolesClaimTransformation<TaxiDriverDBContext, UserEntity, TaxiAuthorityUserEntity, UserInfoCache>
    {
        public TaxiDriverRolesClaimTransformation(IClaimsReaderClient claimsReaderClient, TaxiDriverDBContext dbContext, ICacheManager cacheManager, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) :
            base(claimsReaderClient, dbContext, cacheManager, httpContextAccessor, configuration)
        {
        }

        protected override async Task AddAdditionalClaims(ClaimsPrincipal principal)
        {
            await Task.CompletedTask;
        }
    }
}