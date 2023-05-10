using Asf.Taxi.BusinessLogic.Models.FinancialAdministrations;
using Asf.Taxi.DAL.Entities;
using SmartTech.Infrastructure.Extensions;
using System.Linq.Expressions;

namespace Asf.Taxi.BusinessLogic.Mapper
{
    public static class FinancialAdministrationMapper
    {
        public static FinancialAdministrationEntity Map(this FinancialAdministrationWrite write, long authorityId, long licenseeId) =>
            new()
            {
                AuthorityId = authorityId,
                LicenseeId = licenseeId,
                LegalPersonId = write.LegalPersonId!.Value,
                Deleted = false
            };

        public static FinancialAdministration MapFinancialDetail(this FinancialAdministrationEntity entity, IEnumerable<TaxiDriverEntity> drivers) =>
            new()
            {
                Id = entity.Id,
                LegalPersonId = entity.LegalPersonId,
                LegalPersonDisplayName = entity.LegalPerson!.DisplayName,
                Drivers = drivers.Select(f => new PersonAutocompleteBase
                {
                    Id = f.Id,
                    DisplayName = f.DisplayName,
                }).ToList()
            };

        internal static Expression<Func<FinancialAdministrationEntity, dynamic>> MapSortCriteria(this FinancialAdministrationFilterCriteria dfc) =>
            dfc.KeySelector?.FirstCharToUpper() switch
            {
                _ => x => x.Id
            };
    }
}