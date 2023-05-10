using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class AuditWrite
	{
		[Required]
		public long? ItemId { get; set; }

		[Required]
		public ItemTypes? ItemType { get; set; }

		[Required]
		public OperationTypes? OperationType { get; set; }

		public string? MemoLine { get; set; }

		public string? OldPathItem { get; set; }
	}
}