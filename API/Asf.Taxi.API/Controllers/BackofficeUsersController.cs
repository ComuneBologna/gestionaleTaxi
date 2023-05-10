using Asf.Taxi.BusinessLogic;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using SmartTech.Common;
using SmartTech.Common.API;
using SmartTech.Common.ApplicationUsers;
using SmartTech.Common.ApplicationUsers.Models;
using SmartTech.Common.Web.Security;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.API.Controllers
{
	public class BackofficeUsersController : APIControllerBase
	{
		readonly IUsersService _usersService;
		readonly ITaxiDriversService _driversService;

		public BackofficeUsersController(IUsersService cardManagerUsersService, ITaxiDriversService driversService)
		{
			_driversService = driversService;
			_usersService = cardManagerUsersService;
		}

		[HttpGet]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, BasePermissions.Tenant_Admin)]
		public async Task<FilterResult<BackofficeUser>> SearchBackofficeUsers([FromQuery] UsersSearchCriteria search) =>
			await _usersService.GetUsers<UsersSearchCriteria, BackofficeUser>(search);

		[HttpGet("{id}")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, BasePermissions.Tenant_Admin)]
		public async Task<BackofficeUser?> GetBackofficeUserById(Guid id) =>
			(await _usersService.GetUsers<UsersSearchCriteria, BackofficeUser>(new UsersSearchCriteria
			{
				SmartPAUserId = id
			})).Items.FirstOrDefault();

		[HttpPost]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, BasePermissions.Tenant_Admin)]
		public async Task<BackofficeUser> AddBackofficeUser([FromBody] BackofficeUserWrite user) => await _usersService.AddUser<BackofficeUserWrite, BackofficeUser>(user);

		[HttpPut("{id}")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, BasePermissions.Tenant_Admin)]
		public async Task<BackofficeUser> EditBackofficeUser(Guid id, [FromBody] BackofficeUserUpdate user) =>
			await _usersService.EditUser<BackofficeUserUpdate, BackofficeUser>(id, user);

		[HttpDelete("{id}")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, BasePermissions.Tenant_Admin)]
		public async Task DeleteBackofficeUser(Guid id) => await _usersService.DeleteUser(id);

		[HttpGet("drivers")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator, BasePermissions.Tenant_Admin)]
		public async Task<FilterResult<TaxiDriverInfo>> GetAllDrivers([FromQuery] TaxiDriversFilterCriteria filterCriteria) =>
			await _driversService.SearchDrivers(filterCriteria);
	}
}