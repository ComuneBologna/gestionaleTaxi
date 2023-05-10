namespace Asf.Taxi.BusinessLogic.Models.Vehicles
{
	public class VehicleHistory : VehicleInfo
	{
		public long VehicleId { get; set; }

		public DateTime EndDate { get; set; }
	}
}