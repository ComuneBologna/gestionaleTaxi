using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class VehicleWrite
	{
		[Required]
		[Label(ResourcesConst.Model)]
		public string? Model { get; set; }

		[Required]
		[Label(ResourcesConst.LicensePlate)]
		public string? LicensePlate { get; set; }

		[Required]
		[Label(ResourcesConst.CarFuelType)]
		public CarFuelTypes? CarFuelType { get; set; }

		[Required]
		[DateLessOrEqualThan(nameof(InUseSince))]
		[Label(ResourcesConst.VehicleRegistrationDate)]
		public DateTime? RegistrationDate { get; set; }

		[Required]
		[Label(ResourcesConst.InUseSince)]
		public DateTime? InUseSince { get; set; }
	}
}