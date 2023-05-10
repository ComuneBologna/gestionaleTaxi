namespace Asf.Taxi.API.Models
{
    public class LicenseeVariation
    {
        public long Id { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string? Status { get; set; }

        public string? Acronym { get; set; }

        public string? ShiftName { get; set; }

        public string? SubShiftName { get; set; }

        public string? TaxiDriverAssociationName { get; set; }

        public bool IsFamilyCollaboration { get; set; }

        public string? LicenseeNote { get; set; }

        public string? ActivityExpiredOnCause { get; set; }

        public string? Note { get; set; }

        public DateTime Date { get; set; }

        public string? GarageAddress { get; set; }

        public bool IsFinancialAdministration { get; set; }

    }
}