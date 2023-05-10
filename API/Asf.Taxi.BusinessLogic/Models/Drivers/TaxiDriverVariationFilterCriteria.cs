using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models.Drivers
{
	public class TaxiDriverVariationFilterCriteria : FilterCriteria
	{
		public long? TaxiDriverId { get; set; }

		public List<long> TaxiDriverIds { get; set; } = new List<long>();

		public long? LicenseeId { get; set; }

		public List<long> LicenseeIds { get; set; } = new List<long>();

		public DateTime? VariationDateFrom { get; set; }

		public DateTime? VariationDateTo { get; set; }

		public string? Note { get; set; }
	}
}