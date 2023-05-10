using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models.Requests
{
    public class RequestsRegisterFilterCriteria : FilterCriteria
    {
        public long? Id { get; set; }

        public long? LicenseeId { get; set; }

        public long? TemplateId { get; set; }

        public string? Description { get; set; }

        public DateTime? LastUpdateFrom { get; set; }

        public DateTime? LastUpdateTo { get; set; }

        public ExecutiveDigitalSignStatus? ExecutiveDigitalSignStatus { get; set; }

        public LicenseeTypes? LicenseeType { get; set; }

        public string? LicenseeNumber { get; set; }
    }
}