namespace Asf.Taxi.BusinessLogic.Models.Requests
{
    public class AdditionalInformation
    {
        public string? Year { get; set; }

        public DateTime? Date { get; set; }

        public string? Months { get; set; }

        public string? Day { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string? ProtocolNumber { get; set; }

        public DateTime? ProtocolDate { get; set; }

        public string? Note { get; set; }

        public string? CollaboratorRelationship { get; set; }

        public string? FreeText { get; set; }

        public string? InternalProtocol { get; set; }

        public string? InternalProtocolNumber { get; set; }

        public DateTime? InternalProtocolDate { get; set; }
    }
}