using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models.Vehicles
{
	public class VehicleVariationFilterCriteria : FilterCriteria
	{
		public long? Id { get; set; }

		public List<long> Ids { get; set; } = new List<long>();

		public long? LicenseeId { get; set; }

		public List<long> LicenseeIds { get; set; } = new List<long>();

		public DateTime? VariationDateFrom { get; set; }

		public DateTime? VariationDateTo { get; set; }

		public CarFuelTypes? CarFuelType { get; set; }

		public string? Note { get; set; }
	}
}