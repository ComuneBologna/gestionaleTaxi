using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Common.Models;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
    public class MultipleDigitalSignWrite
    {
        [MinLength(1)]
        [Label(ResourcesConst.Request)]
        public IEnumerable<long> RequestRegisterIds { get; set; } = new List<long>();

        public DigitalSignOTPCredential? Credential { get; set; }
    }
}
