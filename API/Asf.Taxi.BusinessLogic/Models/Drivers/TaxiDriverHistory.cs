namespace Asf.Taxi.BusinessLogic.Models.Drivers
{
	public class TaxiDriverHistory : TaxiDriverInfo
	{
		public long TaxiDriverId { get; set; }

		public DateTime EndDate { get; set; }
	}
}