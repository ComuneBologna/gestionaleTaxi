using Asf.Taxi.DAL.Enums;

namespace Asf.Taxi.BusinessLogic.Models.Requests
{
    public class RequestRegister: RequestRegisterInfo
    {
        public long LicenseeId { get; set; }

        public long TemplateId { get; set; }
        
        public long? ProtocolNumber { get; set; }

        public DateTime? ProtocolDate { get; set; }

        public bool IsSigned { get; set; }

        public ExecutiveDigitalSignStatus ExecutiveDigitalSignStatus { get; set; }
    }
}