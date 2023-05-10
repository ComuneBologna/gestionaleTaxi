using Asf.Taxi.BusinessLogic.Models;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Services
{
	public interface ITaxiDriverAssociationsService
	{
		Task<FilterResult<TaxiDriverAssociation>> SearchTaxiDriverAssociations(TaxiDriverAssociationFilterCriteria filterCriteria);

		Task<long> AddTaxiDriverAssociation(TaxiDriverAssociationWrite associationWrite);

		Task EditTaxiDriverAssociation(TaxiDriverAssociationWrite associationWrite, long id);

		Task DeleteTaxiDriverAssociation(long id);
	}
}