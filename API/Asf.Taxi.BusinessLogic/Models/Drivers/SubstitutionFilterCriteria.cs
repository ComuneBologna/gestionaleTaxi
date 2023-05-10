using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class SubstitutionFilterCriteria : FilterCriteria
	{
		public long? Id { get; set; }

		public long? SubstituteDriverId { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public SubstitutionStatus? SubstitutionStatus { get; set; }
	}
}