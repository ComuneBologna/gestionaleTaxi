using SmartTech.Common.Web.Security;

namespace Asf.Taxi.API.Authentication
{
	class TaxiAuthenticationConstants : AuthenticationConstants
	{
		const string _driverId = "DriverId";

		protected TaxiAuthenticationConstants(string name) : base(name)
		{
		}

		public static TaxiAuthenticationConstants DriverId => new (_driverId);
	}
}