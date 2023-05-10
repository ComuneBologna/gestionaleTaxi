using Asf.Taxi.BusinessLogic;
using SmartTech.Common.Web.Security;
using System.Security.Claims;
using System.Security.Principal;

namespace Asf.Taxi.API.Authentication
{
	public static class ClaimsPrincipalExtensions
	{
		public static long? DriverId(this ClaimsPrincipal principal) => principal?.Identity?.DriverId();

		public static long? DriverId(this IIdentity identity) => ((ClaimsIdentity)identity).DriverId();

		public static long? DriverId(this ClaimsIdentity identity)
		{
			var claim = identity.FindFirst(TaxiAuthenticationConstants.DriverId.Name);

			return !string.IsNullOrWhiteSpace(claim?.Value) ? (long?)Convert.ToInt64(claim.Value) : null;
		}
		
		public static bool IsExecutive(this ClaimsPrincipal principal) => principal.FindAll(AuthenticationConstants.Permission.Name)?.Any(a => a.Value == TaxiPermissions.Taxi_Executive) ?? false;
	}
}