using Asf.Taxi.BusinessLogic;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using SmartTech.Common.API;
using SmartTech.Common.Web.Security;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.API.Controllers
{
	public class TaxiDriverAssociationsController : APIControllerBase
	{
		readonly ITaxiDriverAssociationsService _associationsService;

		public TaxiDriverAssociationsController(ITaxiDriverAssociationsService associationsService)
		{
			_associationsService = associationsService;
		}

		[HttpPost]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task<long> Post([FromBody] TaxiDriverAssociationWrite associationWrite) =>
			await _associationsService.AddTaxiDriverAssociation(associationWrite);

		[HttpPut("{id}")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task Put(long id, [FromBody] TaxiDriverAssociationWrite associationWrite) =>
			await _associationsService.EditTaxiDriverAssociation(associationWrite, id);

		[HttpGet("{id}")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task<TaxiDriverAssociation?> Get(long id) =>
			(await _associationsService.SearchTaxiDriverAssociations(new TaxiDriverAssociationFilterCriteria
			{
				Id = id
			})).Items.FirstOrDefault();

		[HttpGet]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task<FilterResult<TaxiDriverAssociation>> GetAll([FromQuery] TaxiDriverAssociationFilterCriteria search) =>
			await _associationsService.SearchTaxiDriverAssociations(search);

		[HttpDelete("{id}")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task Delete(long id) =>
			await _associationsService.DeleteTaxiDriverAssociation(id);
	}
}