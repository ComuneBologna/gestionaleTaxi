namespace Asf.Taxi.BusinessLogic.Models.Licensees
{
	public class LicenseeVariationFilterCriteria : LicenseesFilterCriteria
	{
		public DateTime? VariationDateFrom { get; set; }

		public DateTime? VariationDateTo { get; set; }

		public string? VariationNote { get; set; }
	}
}