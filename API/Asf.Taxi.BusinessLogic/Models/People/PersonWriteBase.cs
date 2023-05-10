using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
    public class PersonWriteBase : PersonBase
    {
        [Label(ResourcesConst.PhoneNumber)]
        [RegularExpression(@"^[+]{0,1}([(][0-9]{1,4}[)]){0,1}[-\s./0-9]*$", ResourcesConst.PhoneNumberFormat)]
        public string? PhoneNumber { get; set; }
    }
}
