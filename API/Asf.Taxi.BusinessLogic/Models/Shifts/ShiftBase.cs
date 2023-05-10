using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class ShiftBase
	{
		[Required]
		[Label(ResourcesConst.FirstName)]
		public string? Name { get; set; }

		[Required]
		[MaxNumber(24, include: true)]
		[Label(ResourcesConst.DurationInHour)]
		public byte? DurationInHour { get; set; }
	}
}