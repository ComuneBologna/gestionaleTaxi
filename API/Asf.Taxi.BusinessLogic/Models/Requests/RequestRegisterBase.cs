using Asf.Taxi.DAL.Enums;

namespace Asf.Taxi.BusinessLogic.Models.Requests
{
    public class RequestRegisterBase
    {
        public long Id { get; set; }

        public string? TemplateDescription { get; set; }

        public string? LicenseeNumber { get; set; }

        public LicenseeTypes LicenseeType { get; set; }
    }
}