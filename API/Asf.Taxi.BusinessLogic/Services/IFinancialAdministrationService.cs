using Asf.Taxi.BusinessLogic.Models.FinancialAdministrations;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Services
{
    public interface IFinancialAdministrationService
    {
        Task<FilterResult<FinancialAdministration>> SearchFinancialAdministrations(long licenseeId, FinancialAdministrationFilterCriteria filterCriteria);

        Task<long> AddFinancialAdministration(long licenseeId, FinancialAdministrationWrite write);

        Task EditFinancialAdministration(long licenseeId, FinancialAdministrationWrite write);

        Task DeleteFinancialAdministration(long licenseeId);
    }
}
