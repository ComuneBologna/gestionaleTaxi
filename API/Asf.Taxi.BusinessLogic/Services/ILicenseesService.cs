using Asf.Taxi.BusinessLogic.Models.Licensees;
using Asf.Taxi.BusinessLogic.Models.LicenseesIssuingOffice;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Services
{
	public interface ILicenseesService
	{
		Task<List<string?>> GetRecipientsAutocomplete(long licenseId, string mail);

        Task<List<string?>> GetRecipients(long licenseId);

        Task<FilterResult<T>> SearchLicensees<T>(LicenseesFilterCriteria filterCriteria) where T : LicenseeBase;

		Task<long> AddLicensee<T>(T licenseeWrite) where T : LicenseeWrite;

		Task UpdateLicensee<T>(long id, T licenseeWrite) where T : LicenseeWrite;

		Task<FilterResult<LicenseeHistory>> SearchLicenseeVariations(LicenseeVariationFilterCriteria filterCriteria);

		Task<FilterResult<LicenseesIssuingOffice>> SearchLicenseesIssuingOffices(LicenseesIssuingOfficesFilterCriteria filterCriteria);

		Task<long> AddLicenseesIssuingOffice(LicenseesIssuingOfficeWrite licenseeIssuingOfficeWrite);

		Task UpdateLicenseesIssuingOffice(long id, LicenseesIssuingOfficeWrite licenseeIssuingOfficeWrite);

		Task DeleteLicenseesIssuingOffice(long licenseeId);

		Task RenewLicensee(long licenseeId);
	}
}