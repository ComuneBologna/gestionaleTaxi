namespace Asf.Taxi.BusinessLogic.Models.Licensees
{
	public class LicenseeHistory : LicenseeInfo
	{
		public long LicenseeId { get; set; }

		public string? Note { get; set; }

		public string? LicenseeNote { get; set; }

		public string? Acronym { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime VariationEndDate { get; set; }

		public long? LicenseesIssuingOfficeId { get; set; }

		public string? LicenseesIssuingOfficeDescription { get; set; }

		public string? GarageAddress { get; set; }

		public bool IsFinancialAdministration { get; set; }

		public bool IsFamilyCollaboration { get; set; }

		public long? FinancialAdministrationId { get; set; }

		public string? ActivityExpiredOnCause { get; set; }
	}
}