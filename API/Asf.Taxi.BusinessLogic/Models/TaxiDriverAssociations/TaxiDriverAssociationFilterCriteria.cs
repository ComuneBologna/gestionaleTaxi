using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class TaxiDriverAssociationFilterCriteria : FilterCriteria
	{
		public long? Id { get; set; }

		public long? LicenseeId { get; set; }

		public string? Name { get; set; }

		public string? FiscalCode { get; set; }

		internal bool? IsDeleted { get; set; }
	}
}