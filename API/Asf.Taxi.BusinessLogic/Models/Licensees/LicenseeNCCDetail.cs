namespace Asf.Taxi.BusinessLogic.Models.Licensees
{
	public class LicenseeNCCDetail : LicenseeNCCWrite
	{
		public long Id { get; set; }

		public long? VehicleId { get; set; }

		public string? TaxiDriverAssociationName { get; set; }

		public DateTime EndDate { get; set; }

		public string? LicenseesIssuingOfficeDescription { get; set; }
	}
}