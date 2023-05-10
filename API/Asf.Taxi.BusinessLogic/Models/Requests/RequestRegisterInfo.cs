namespace Asf.Taxi.BusinessLogic.Models.Requests
{
    public class RequestRegisterInfo : RequestRegisterBase
    {
        public string? TemplateFileName { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}