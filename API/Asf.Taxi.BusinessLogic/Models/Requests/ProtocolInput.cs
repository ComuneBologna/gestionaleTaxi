using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.Requests
{
    public class ProtocolInput
    {
        [Required]
        [Label(ResourcesConst.Subject)]
        public string? Subject { get; set; }

        [Required]
        [Label(ResourcesConst.Recipient)]
        public string? Recipient { get; set; }
    }
}