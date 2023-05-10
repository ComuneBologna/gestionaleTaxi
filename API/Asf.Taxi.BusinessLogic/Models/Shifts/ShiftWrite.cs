using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class ShiftWrite : ShiftBase
	{
		[Required]
		[Label(ResourcesConst.IsHandicapMode)]
		public bool? IsHandicapMode { get; set; }

		[MaxNumber(24, include: true)]
		[RequiredIf(nameof(IsHandicapMode))]
		[Label(ResourcesConst.HandicapBeforeInHour)]
		public byte? HandicapBeforeInHour { get; set; }

		[MaxNumber(24, include: true)]
		[RequiredIf(nameof(IsHandicapMode))]
		[Label(ResourcesConst.HandicapAfterInHour)]
		public byte? HandicapAfterInHour { get; set; }

		[Required]
		[MaxNumber(24, include: true)]
		[Label(ResourcesConst.BreakInHours)]
		public byte? BreakInHours { get; set; }

		[Required]
		[MaxNumber(24, include: true)]
		[Label(ResourcesConst.BreakThresholdActivationInHour)]
		public byte? BreakThresholdActivationInHour { get; set; }

		[Required]
		[MaxNumber(31, include: true)]
		[Label(ResourcesConst.RestDayFrequencyInDays)]
		public byte? RestDayFrequencyInDays { get; set; }

		[Required]
		[RequiredItems(1)]
		[Label(ResourcesConst.SubShifts)]
		public List<SubShift> SubShifts { get; set; } = new();
	}
}