using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.API.Models
{
    public class ProxyDocumentSearchCriteriaModel : FilterCriteria
    {
        public string? Status { get; set; }

        public string? ProtocolYear { get; set; }

        public string? ProtocolNumber { get; set; }

        public string? Title { get; set; }

        public string? Type { get; set; }
    }
}