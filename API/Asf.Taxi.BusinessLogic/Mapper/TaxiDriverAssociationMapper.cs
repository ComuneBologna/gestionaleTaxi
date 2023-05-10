using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.DAL.Entities;

namespace Asf.Taxi.BusinessLogic.Mapper
{
	public static class TaxiDriverAssociationMapper
	{
		public static TaxiDriverAssociation Map(this TaxiDriverAssociationEntity association) =>
			new()
			{
				Id = association.Id,
				TelephoneNumber = association.TelephoneNumber,
				Email = association.Email,
				FiscalCode = association.FiscalCode,
				IsDeleted = association.IsDeleted,
				Name = association.Name
			};

		public static TaxiDriverAssociationEntity Map(this TaxiDriverAssociationWrite association, long authorityId) =>
			new()
			{
				AuthorityId = authorityId,
				Email = association.Email,
				FiscalCode = association.FiscalCode,
				IsDeleted = false,
				Name = association.Name,
				TelephoneNumber = association.TelephoneNumber
			};
	}
}