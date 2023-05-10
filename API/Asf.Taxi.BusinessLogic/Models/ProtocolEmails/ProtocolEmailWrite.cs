using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.ProtocolEmails
{
    public class ProtocolEmailWrite : ProtocolEmailBase
    {
        [MaxLength(1024)]
        public string? Description { get; set; }
    }
}
