using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models.FinancialAdministrations
{
    public class FinancialAdministrationFilterCriteria : FilterCriteria
    {
        public long? Id { get; set; }

        public IEnumerable<long> Ids { get; set; } = new List<long>();

        public long? LegalPersonId { get; set; }

        public string? LegalPersonDisplayName { get; set; }
    }
}