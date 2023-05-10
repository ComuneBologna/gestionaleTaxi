using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Models.Drivers;
using Asf.Taxi.DAL.Enums;
using SmartTech.Common.Enums;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Services
{
    public interface ITaxiDriversService
    {
        Task<FilterResult<TaxiDriverInfo>> SearchDrivers(TaxiDriversFilterCriteria filterCriteria, DriverTypes? type = null, bool? isAutoComplete = null);

        Task<TaxiDriverInfo> AddDriver(long licenseeId, TaxiDriverWrite write, DriverTypes type);

        Task UpsertDriver(long licenseeId, TaxiDriverWrite write, DriverTypes type);

        Task<bool> CheckAllDocumentsExists(long licenseeId, long taxiDriverId);

        Task<TaxiDriverInfo> AddPerson<T>(T write, PersonType type) where T : PersonWriteBase;

        Task EditPerson<T>(long personId, T personWrite, PersonType type) where T : PersonWriteBase;

        Task<long> AddSubstitution(long licenseeId, SubstitutionWrite substitution);

        Task EditSubstitution(long licenseeId, long id, SubstitutionEdit substitution);

        Task DeleteSubstitution(long licenseeId, long substitutionId);

        Task<FilterResult<SubstitutionInfo>> SearchSubstitutions(long licenseeId, SubstitutionFilterCriteria criteria);

        Task<FilterResult<T>> SearchDriverVariations<T>(TaxiDriverVariationFilterCriteria filterCriteria, DriverTypes type) where T : TaxiDriverInfo;
    }
}