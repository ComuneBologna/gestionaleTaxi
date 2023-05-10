using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Models
{
    public class CredentialUserFilterCriteria : FilterCriteria
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}