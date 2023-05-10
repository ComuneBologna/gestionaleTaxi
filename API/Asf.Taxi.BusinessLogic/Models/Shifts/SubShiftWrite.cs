using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class SubShiftWrite
	{
		[Required]
		[Label(ResourcesConst.FirstName)]
		public string? Name { get; set; }

		[Required]
		[Label(ResourcesConst.RestDay)]
		public DayOfWeek? RestDay { get; set; }
	}
}