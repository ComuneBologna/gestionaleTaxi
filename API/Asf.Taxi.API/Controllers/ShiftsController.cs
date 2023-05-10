using Asf.Taxi.BusinessLogic;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using SmartTech.Common.API;
using SmartTech.Common.Web.Security;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.API.Controllers
{
	public class ShiftsController : APIControllerBase
	{
		readonly IShiftsService _shiftService;

		public ShiftsController(IShiftsService shiftService)
		{
			_shiftService = shiftService;
		}

		[HttpPost]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task<long> Post([FromBody] ShiftWrite shift) =>
			await _shiftService.AddShift(shift);

		[HttpPut("{shiftId}")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task Put(long shiftId, [FromBody] ShiftWrite shift) =>
			await _shiftService.EditShift(shiftId, shift);

		[HttpDelete("{shiftId}")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task Delete(long shiftId) => await _shiftService.DeleteShift(shiftId);

		[HttpGet]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task<FilterResult<ShiftInfo>> Get([FromQuery] ShiftsSearchCriteria search) =>
			await _shiftService.SearchShifts<ShiftInfo>(search);

		[HttpGet("{shiftId}")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task<Shift?> Get(long shiftId) =>
			(await _shiftService.SearchShifts<Shift>(new ShiftsSearchCriteria
			{
				Id = shiftId
			})).Items.FirstOrDefault();
	}
}