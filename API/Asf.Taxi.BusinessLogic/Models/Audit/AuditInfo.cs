namespace Asf.Taxi.BusinessLogic.Models
{
	public class AuditInfo : AuditWrite
	{
		public string? Username { get; set; }

		public DateTime CreatedAt { get; set; }

		public string? PathOldItem { get; set; }
	}
}