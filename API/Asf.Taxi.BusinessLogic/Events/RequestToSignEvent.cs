namespace Asf.Taxi.BusinessLogic.Events
{
    public sealed class RequestToSignEvent: TaxiDriverEventBase
    {
        public List<long> RequestIds { get; set; } = new();

        public string? ExecutiveEmail { get; set; }
    }
}