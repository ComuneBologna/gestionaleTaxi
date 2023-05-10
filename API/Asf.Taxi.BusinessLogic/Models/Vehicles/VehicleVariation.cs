using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class VehicleVariation : VehicleWrite
	{
		[Required]
		[Label(ResourcesConst.VariationNote)]
		public string? Note { get; set; }
	}
}