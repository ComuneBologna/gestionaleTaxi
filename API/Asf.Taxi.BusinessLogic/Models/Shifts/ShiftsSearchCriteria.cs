using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class ShiftsSearchCriteria : FilterCriteria
	{
		public long? Id { get; set; }

		public byte? DurationInHour { get; set; }

		public string? Name { get; set; }
	}
}