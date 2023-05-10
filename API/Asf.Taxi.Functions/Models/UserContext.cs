using Asf.Taxi.BusinessLogic;
using SmartTech.Common.Authentication;

namespace Asf.Taxi.Functions.Models
{
	class UserContext : ITaxiUserContext, ISettableUserContext
	{
		Guid _userId;
		Guid _tenantId;
		long _authorityid;
		string _displayName;

		long? ITaxiUserContext.DriverId => default;

		Guid IUserContext.TenantId => _tenantId;

		Guid? IUserContext.SmartPAUserId => _userId;

		long? IUserContext.ApplicationId => default;

		long IUserContext.AuthorityId => _authorityid;

		string IUserContext.DisplayName => _displayName;

		bool IUserContext.IsSpidUser => default;

		public bool IsTenantAdmin => default;

		public bool IsApplication => default;

		bool ITaxiUserContext.IsExecutive => default;

		void ISettableUserContext.SetAuthorityId(long authorityId) => _authorityid = authorityId;

		void ISettableUserContext.SetSmartPAUserId(Guid userId) => _userId = userId;

		void ISettableUserContext.SetTenantId(Guid tenantId) => _tenantId = tenantId;

		public void SetDisplayName(string displayName) => _displayName = displayName;
	}
}