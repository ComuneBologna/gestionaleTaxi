using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.API.Models
{
    public class RequestsRegisterFilterCriteriaAPI : FilterCriteria
    {
        public long? Id { get; set; }

        public string? Description { get; set; }
    }
}