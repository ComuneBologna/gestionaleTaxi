namespace Asf.Taxi.BusinessLogic.Models.Licensees
{
	public class LicenseeTaxiDetail: LicenseeTaxiWrite
	{
		public long Id { get; set; }

		public long? VehicleId { get; set; }

		public string? TaxiDriverAssociationName { get; set; }

		public string? SubShiftName { get; set; }

		public string? ShiftName { get; set; }

		public DateTime EndDate { get; set; }

		public string? LicenseesIssuingOfficeDescription { get; set; }
	}
}