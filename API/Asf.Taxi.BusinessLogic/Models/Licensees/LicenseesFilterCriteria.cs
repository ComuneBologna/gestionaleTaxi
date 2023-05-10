using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models.Licensees
{
    public class LicenseesFilterCriteria : FilterCriteria
    {
        public long? Id { get; set; }

        public List<long> Ids { get; set; } = new List<long>();

        public string? Number { get; set; }

        public LicenseeStatus? Status { get; set; }

        public LicenseeTypes? Type { get; set; }

        public string? VehiclePlate { get; set; }

        public DateTime? EndFrom { get; set; }

        public DateTime? EndTo { get; set; }

        public DateTime? ReleaseDateFrom { get; set; }

        public DateTime? ReleaseDateTo { get; set; }

        public long? ShiftId { get; set; }

        public long? SubShiftId { get; set; }

        public string? Note { get; set; }

        public string? ShiftName { get; set; }

        public string? SubShiftName { get; set; }

        public long? TaxiDriverAssociationId { get; set; }

        public CarFuelTypes? CarFuelType { get; set; }

        public string? TaxiDriverLastName { get; set; }

        public bool? IsFamilyCollaboration { get; set; }

        public string? GarageAddress { get; set; }

        public bool? IsFinancialAdministration { get; set; }

        public bool? IsSubstitution { get; set; }

        public bool? HasActiveSubstitution { get; set; }

        public string? Acronym { get; set; }
    }
}