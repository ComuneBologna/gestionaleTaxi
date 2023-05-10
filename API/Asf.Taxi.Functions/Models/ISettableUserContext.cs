using Asf.Taxi.BusinessLogic;

namespace Asf.Taxi.Functions.Models
{
	public interface ISettableUserContext : ITaxiUserContext
	{
		public void SetTenantId(Guid tenantId);

		public void SetAuthorityId(long authorityId);

		public void SetSmartPAUserId(Guid userId);

		public void SetDisplayName(string displayName);
	}
}