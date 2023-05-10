using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class AuditsFilterCriteria : FilterCriteria
	{
		public long? ItemId { get; set; }

		public ItemTypes? ItemType { get; set; }

		public OperationTypes? OperationType { get; set; }

		public DateTime? VariationDateFrom { get; set; }

		public DateTime? VariationDateTo { get; set; }
	}
}