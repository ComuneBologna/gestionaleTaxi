namespace Asf.Taxi.BusinessLogic.Models
{
	public class TaxiDriverAssociation : TaxiDriverAssociationWrite
	{
		public long Id { get; set; }

		public bool IsDeleted { get; set; }
	}
}