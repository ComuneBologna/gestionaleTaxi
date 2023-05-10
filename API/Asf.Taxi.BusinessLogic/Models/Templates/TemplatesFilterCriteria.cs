using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models.Templates
{
    public class TemplatesFilterCriteria : FilterCriteria
    {
        public long? Id { get; set; }
        
        public string? Description { get; set; }

        public string? FileName { get; set; }

        public List<long> Ids { get; set; } = new List<long>();
    }
}