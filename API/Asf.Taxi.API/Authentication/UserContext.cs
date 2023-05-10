using Asf.Taxi.BusinessLogic;
using SmartTech.Common.Authentication;
using SmartTech.Common.Web.Security.Extensions;

namespace Asf.Taxi.API.Authentication
{
	public class UserContext : ITaxiUserContext
	{
		readonly IHttpContextAccessor _httpContextAccessor;

		public UserContext(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

		bool IUserContext.IsTenantAdmin => _httpContextAccessor.HttpContext!.User.IsTenantAdmin();

		Guid IUserContext.TenantId => _httpContextAccessor.HttpContext!.User.TenantId();

		Guid? IUserContext.SmartPAUserId => _httpContextAccessor.HttpContext!.User.SmartPAUserId();

		long? ITaxiUserContext.DriverId => _httpContextAccessor.HttpContext!.User.DriverId();

		string IUserContext.DisplayName => _httpContextAccessor.HttpContext!.User.DisplayName();

		long IUserContext.AuthorityId => _httpContextAccessor.HttpContext!.User.AuthorityId();

		long? IUserContext.ApplicationId => _httpContextAccessor.HttpContext!.User.ApplicationId();

		bool IUserContext.IsSpidUser => _httpContextAccessor.HttpContext!.User.IsSpidUser();

		bool IUserContext.IsApplication => _httpContextAccessor.HttpContext!.User.IsApplication();

		bool ITaxiUserContext.IsExecutive => _httpContextAccessor.HttpContext!.User.IsExecutive();
	}
}