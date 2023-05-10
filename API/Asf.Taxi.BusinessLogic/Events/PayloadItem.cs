using Asf.Taxi.BusinessLogic.Models;

namespace Asf.Taxi.BusinessLogic.Events
{
	public class PayloadItem
	{
		public AuditWrite? Audit { get; set; }

		public object? Data { get; set; }
	}
}