using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
    public class LegalPersonWrite : PersonWriteBase
    {
        [Required]
        [Label(ResourcesConst.Nominative)]
        public string? Nominative { get; set; }

        [Required]
        [Label(ResourcesConst.VATNumber)]
        public string? VATNumber { get; set; }
    }
}
