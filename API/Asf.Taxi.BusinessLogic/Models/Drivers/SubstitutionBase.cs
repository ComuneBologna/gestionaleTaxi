using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class SubstitutionBase
	{
		[Required]
		[DateLessOrEqualThan(nameof(EndDate))]
		[Label(ResourcesConst.SubstitutionStartDate)]
		public DateTime? StartDate { get; set; }

		[Required]
		[Label(ResourcesConst.SubstitutionEndDate)]
		public DateTime? EndDate { get; set; }

		public string? Note { get; set; }
	}
}