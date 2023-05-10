using Asf.Taxi.DAL.Enums;

namespace Asf.Taxi.BusinessLogic.Models.Licensees
{
	public class LicenseeInfo : LicenseeBase
	{
		public long Id { get; set; }

		public string? VehiclePlate { get; set; }

		public string? DriverDisplayName { get; set; }

		public string? TaxiDriverAssociationName { get; set; }

		public string? SubShiftName { get; set; }

		public string? ShiftName { get; set; }

		public DateTime? ReleaseDate { get; set; }

		public CarFuelTypes? CarFuelType { get; set; }

		public bool OwnerAllDocuments { get; set; }

		public bool CollaboratorAllDocuments { get; set; }

		public bool IsExpiring { get; set; }
	}
}