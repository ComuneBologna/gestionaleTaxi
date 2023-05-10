using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.ProtocolEmails
{
    public class ProtocolEmailBase
    {
        [Required]
        [MaxLength(512)]
        [RegularExpression(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$", ResourcesConst.Email)]
        public string? Email { get; set; }
    }
}
