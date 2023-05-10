using Asf.Taxi.DAL.Enums;

namespace Asf.Taxi.BusinessLogic.Events
{
	public class TaxiDriverFolderCreateEvent : TaxiDriverEventBase
	{
		public string? LicenseeNumber { get; set; }

		public LicenseeTypes LicenseeType { get; set; }

		public long LicenseeId { get; set; }

		public string? RolePath { get; set; }
	}
}