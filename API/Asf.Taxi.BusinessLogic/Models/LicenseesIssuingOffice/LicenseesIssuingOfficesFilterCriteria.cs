using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models.LicenseesIssuingOffice
{
    public class LicenseesIssuingOfficesFilterCriteria : FilterCriteria
    {
        public long? Id { get; set; }

        public List<long> Ids { get; set; } = new();

        public string? Description { get; set; }
    }
}