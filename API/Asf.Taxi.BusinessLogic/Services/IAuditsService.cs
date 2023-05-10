using Asf.Taxi.BusinessLogic.Models;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Services
{
	public interface IAuditsService
	{
		Task AddAudit(AuditWrite auditWrite);

		Task<FilterResult<AuditInfo>> SearchAudits(AuditsFilterCriteria filter);
	}
}