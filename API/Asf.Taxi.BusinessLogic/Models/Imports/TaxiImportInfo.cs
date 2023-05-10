using Asf.Taxi.DAL.Enums;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class TaxiImportInfo : TaxiImportBase
	{
		public long Id { get; set; }

		public string? Path { get; set; }

		public ImportStatus Status { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}