using System;

namespace Asf.Taxi.BusinessLogic.Events
{
	public class TaxiDriverEventBase
	{
		public Guid UserId { get; set; }

		public Guid TenantId { get; set; }

		public long AuthorityId { get; set; }
	}
}