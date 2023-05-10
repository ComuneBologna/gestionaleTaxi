using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.FinancialAdministrations
{
    public class FinancialAdministrationWrite : FinancialAdministrationBase
    {
        [Required]
        [Label(ResourcesConst.LegalPerson)]
        public long? LegalPersonId { get; set; }
    }
}
