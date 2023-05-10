namespace Asf.Taxi.BusinessLogic.Events
{
	public class TaxiDriverAuditEvent : TaxiDriverEventBase
	{
		public string? DisplayName { get; set; }

		public PayloadItem? AuditPayload { get; set; }
	}
}