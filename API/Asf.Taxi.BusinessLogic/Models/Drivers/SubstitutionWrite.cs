using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class SubstitutionWrite : SubstitutionBase
	{
		[Required]
		[Label(ResourcesConst.DriverIdTo)]
		public long? SubstituteDriverId { get; set; }
	}
}