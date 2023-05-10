using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.Licensees
{
	public class LicenseeBase
	{
		[Required]
		[Label(ResourcesConst.Number)]
		public string? Number { get; set; }

		[Required]
		[Label(ResourcesConst.StateType)]
		public LicenseeStatus? Status { get; set; }

		[Required]
		[Label(ResourcesConst.LicenseeType)]
		public LicenseeTypes? Type { get; set; }
	}
}