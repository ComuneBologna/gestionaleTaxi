using SmartTech.Common.Enums;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models
{
    public class TaxiDriversFilterCriteria : FilterCriteria
    {
        public long? Id { get; set; }

        public List<long> Ids { get; set; } = new List<long>();

        public long? LicenseeId { get; set; }

        public string? LicenseeNumber { get; set; }

        public string? PersonDescription { get; set; }

        public long? VehicleId { get; set; }

        public string? FiscalCode { get; set; }

        public string? PhoneNumber { get; set; }

        public string? EmailOrPEC { get; set; }

        public PersonType? PersonType { get; set; }
    }
}