using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.Licensees
{
	public class LicenseeWrite : LicenseeBase
	{
		[Required]
		[Label(ResourcesConst.ReleasedBy)]
		public long? LicenseesIssuingOfficeId { get; set; }

		[Required]
		[Label(ResourcesConst.ReleaseDate)]
		public DateTime? ReleaseDate { get; set; }

		public string? ActivityExpiredOnCause { get; set; }

		public long? TaxiDriverAssociationId { get; set; }

		public string? Note { get; set; }

		public string? Acronym { get; set; }

		[Required]
		[Label(ResourcesConst.IsFamilyCollaboration)]
		public bool? IsFamilyCollaboration { get; set; }
	}
}