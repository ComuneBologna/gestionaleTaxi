using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.Licensees
{
	public class LicenseeTaxiVariation : LicenseeTaxiWrite
	{
		[Label(ResourcesConst.VariationNote)]
		public string? VariationNote { get; set; }
	}

	public class LicenseeNCCVariation : LicenseeNCCWrite
	{
		[Label(ResourcesConst.VariationNote)]
		public string? VariationNote { get; set; }
	}
}