using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.FinancialAdministrations
{
    public class FinancialAdministrationBase
    {
        [Required]
        [MinLength(1)]
        [Label(ResourcesConst.Drivers)]
        [SkipChildPropertiesValidation]
        public List<PersonAutocompleteBase> Drivers { get; set; } = new();
    }
}