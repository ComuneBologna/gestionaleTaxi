using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class TaxiImportWrite : TaxiImportBase
	{
		[Required]
		[Label(ResourcesConst.File)]
		public string? AttachmentId { get; set; }
	}
}