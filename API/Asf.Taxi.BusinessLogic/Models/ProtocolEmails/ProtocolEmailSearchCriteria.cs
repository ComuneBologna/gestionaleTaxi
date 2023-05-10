using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models.ProtocolEmails
{
    public class ProtocolEmailSearchCriteria : FilterCriteria
    {
        public long? Id { get; set; }

        public IEnumerable<long> Ids { get; set; } = new List<long>();

        public string? Email { get; set; }

        public string? Description { get; set; }

        public bool? Active { get; set; }
    }
}
