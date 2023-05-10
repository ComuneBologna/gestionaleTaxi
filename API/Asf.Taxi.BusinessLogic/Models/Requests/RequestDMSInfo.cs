namespace Asf.Taxi.BusinessLogic.Models.Requests
{
    public class RequestDMSInfo
    {
        public bool IsSigned { get; set; }

        public DateTime? ProtocolDate { get; set; }
        
        public int? ProtocolNumber { get; set; }

        public string? FileName { get; set; }
    }
}