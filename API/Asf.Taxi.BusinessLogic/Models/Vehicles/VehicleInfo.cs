namespace Asf.Taxi.BusinessLogic.Models
{
	public class VehicleInfo : VehicleWrite
	{
		public long Id { get; set; }

		public long LicenseeId { get; set; }

		public DateTime StartDate { get; set; }

		public string? Note { get; set; }
	}
}