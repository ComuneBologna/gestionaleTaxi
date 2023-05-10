using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Models.Vehicles;
using Asf.Taxi.DAL.Entities;
using SmartTech.Infrastructure.Extensions;
using System.Linq.Expressions;

namespace Asf.Taxi.BusinessLogic.Mapper
{
	static class VehicleMapper
	{
		public static VehicleInfo Map(this VehicleEntity vehicleEntity) =>
			vehicleEntity != default ? new()
			{
				CarFuelType = vehicleEntity.CarFuelType,
				Id = vehicleEntity.Id,
				InUseSince = vehicleEntity.InUseSince,
				Model = vehicleEntity.Model,
				RegistrationDate = vehicleEntity.RegistrationDate,
				LicensePlate = vehicleEntity.LicensePlate,
				LicenseeId = vehicleEntity.LicenseeId,
				StartDate = vehicleEntity.SysStartTime
			} : default;

		public static VehicleEntity Map(this VehicleWrite vehicleWrite, long authorityId, long licenseeId) =>
			new()
			{
				CarFuelType = vehicleWrite.CarFuelType.Value,
				InUseSince = vehicleWrite.InUseSince.Value,
				Model = vehicleWrite.Model,
				RegistrationDate = vehicleWrite.RegistrationDate.Value,
				LicensePlate = vehicleWrite.LicensePlate,
				SysStartTime = DateTime.UtcNow,
				AuthorityId = authorityId,
				LicenseeId = licenseeId
			};
		
		public static Expression<Func<LicenseeHistoryEntity, dynamic>> MapSortCriteria(this VehicleVariationFilterCriteria ofc) =>
			ofc.KeySelector.FirstCharToUpper() switch
			{
				nameof(VehicleHistory.CarFuelType) => x => x.VehicleCarFuelType,
				nameof(VehicleHistory.StartDate) => x => x.SysStartTime,
				nameof(VehicleHistory.VehicleId) => x => x.VehicleId,
				nameof(VehicleHistory.EndDate) => x => x.SysEndTime,
				nameof(VehicleHistory.InUseSince) => x => x.VehicleInUseSince,
				nameof(VehicleHistory.LicenseeId) => x => x.LicenseeId,
				nameof(VehicleHistory.LicensePlate) => x => x.VehicleLicensePlate,
				nameof(VehicleHistory.Model) => x => x.VehicleModel,
				nameof(VehicleHistory.Note) => x => x.Note,
				nameof(VehicleHistory.RegistrationDate) => x => x.VehicleRegistrationDate,
				_ => x => x.Id
			};

		public static T Map<T>(this LicenseeHistoryEntity vehicle) where T : VehicleInfo =>
			typeof(T).Equals(typeof(VehicleHistory)) ?
			new VehicleHistory
			{
				CarFuelType = vehicle.VehicleCarFuelType,
				StartDate = vehicle.SysStartTime,
				Note = vehicle.Note,
				LicenseeId = vehicle.LicenseeId,
				InUseSince = vehicle.VehicleInUseSince,
				Model = vehicle.VehicleModel,
				RegistrationDate = vehicle.VehicleRegistrationDate,
				LicensePlate = vehicle.VehicleLicensePlate,
				Id = vehicle.Id,
				VehicleId = vehicle.VehicleId.GetValueOrDefault(),
				EndDate = vehicle.SysEndTime,
			} as T :
			new VehicleInfo
			{
				CarFuelType = vehicle.VehicleCarFuelType,
				StartDate = vehicle.SysStartTime,
				Note = vehicle.Note,
				LicenseeId = vehicle.LicenseeId,
				InUseSince = vehicle.VehicleInUseSince,
				Model = vehicle.VehicleModel,
				RegistrationDate = vehicle.VehicleRegistrationDate,
				LicensePlate = vehicle.VehicleLicensePlate,
				Id = vehicle.VehicleId.GetValueOrDefault()
			} as T;
	}
}