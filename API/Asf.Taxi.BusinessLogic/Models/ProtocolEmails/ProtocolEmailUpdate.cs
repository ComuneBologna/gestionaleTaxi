using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.ProtocolEmails
{
    public class ProtocolEmailUpdate : ProtocolEmailWrite
    {
        [Required]
        [Label(ResourcesConst.Active)]
        public bool? Active { get; set; }
    }
}
