using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class ImportFilterCriteria : FilterCriteria
	{
		public long? Id { get; set; }

		public List<long> Ids { get; set; } = new();

		public string? Description { get; set; }

		public ImportStatus? Status { get; set; }

		public DateTime? CreatedAtFrom { get; set; }

		public DateTime? CreatedAtTo { get; set; }
	}
}