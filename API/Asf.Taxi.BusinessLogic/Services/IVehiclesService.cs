using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Models.Vehicles;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Services
{
	public interface IVehiclesService
	{
		Task<VehicleInfo> GetVehicle(long licenseeId);

		Task<long> AddVehicle(long licenseeId, VehicleWrite vehicleWrite);

		Task UpsertVehicle(long licenseeId, VehicleWrite vehicleWrite);

		Task<FilterResult<T>> SearchVehicleVariations<T>(VehicleVariationFilterCriteria filterCriteria) where T : VehicleInfo;
	}
}