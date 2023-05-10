using Asf.Taxi.BusinessLogic.Events;
using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.BusinessLogic.Mapper;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Models.Vehicles;
using Asf.Taxi.DAL;
using Asf.Taxi.DAL.Entities;
using Asf.Taxi.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using SmartTech.Common;
using SmartTech.Infrastructure.DataAccessLayer.EFCore;
using SmartTech.Infrastructure.Exceptions;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Search;
using SmartTech.Infrastructure.SystemEvents;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Services
{
	class VehiclesService : IVehiclesService
	{
		readonly ITaxiUserContext _userContext;
		readonly TaxiDriverDBContext _dbContext;
		readonly ISystemEventManager _systemEventManager;

		public VehiclesService(TaxiDriverDBContext dbContext, ITaxiUserContext userContext, ISystemEventManager systemEventManager)
		{
			_dbContext = dbContext;
			_userContext = userContext;
			_systemEventManager = systemEventManager;
		}

		async Task<VehicleInfo> IVehiclesService.GetVehicle(long licenseeId) =>
			(await _dbContext.Vehicles.AsNoTracking()
					.FirstOrDefaultAsync(v => v.AuthorityId == _userContext.AuthorityId &&
												v.LicenseeId == licenseeId)).Map();

		async Task<long> IVehiclesService.AddVehicle(long licenseeId, VehicleWrite vehicleWrite)
		{
			vehicleWrite?.Validate();

			if (await _dbContext.Licensees.AsNoTracking()
						.AnyAsync(l => l.AuthorityId == _userContext.AuthorityId &&
										l.Id == licenseeId && l.Status == LicenseeStatus.Terminated))
				throw new BusinessLogicValidationException(Errors.LicenseeUseStateError);

			if (await _dbContext.Vehicles.AsNoTracking()
						.AnyAsync(v => v.AuthorityId == _userContext.AuthorityId && v.LicenseeId == licenseeId))
				throw new BusinessLogicValidationException("Questa licenza ha già un veicolo assegnato");

			await CheckEntityAlreadyExist(vehicleWrite.LicensePlate);

			var vehicle = vehicleWrite.Map(_userContext.AuthorityId, licenseeId);

			await _dbContext.Vehicles.AddAsync(vehicle);
			await _dbContext.SaveChangesAsync();
			await _systemEventManager.SendAsync(ApplicationsTopics.TaxiDriverEvents, new SystemEventBase
			{
				Payload = new TaxiDriverAuditEvent
				{
					AuthorityId = _userContext.AuthorityId,
					DisplayName = _userContext.DisplayName,
					TenantId = _userContext.TenantId,
					UserId = _userContext.SmartPAUserId!.Value,
					AuditPayload = new PayloadItem
					{
						Audit = new AuditWrite
						{
							ItemId = vehicle.Id,
							ItemType = ItemTypes.Vehicle,
							MemoLine = null,
							OperationType = OperationTypes.Creating
						}
					}
				}.JsonSerialize(false),
				Type = TaxiDriverEventTypes.TaxiDriverAudit
			});

			return vehicle.Id;
		}

		async Task IVehiclesService.UpsertVehicle(long licenseeId, VehicleWrite vehicleWrite)
		{
			vehicleWrite?.Validate();

			if (await _dbContext.Licensees.AsNoTracking()
						.AnyAsync(l => l.AuthorityId == _userContext.AuthorityId &&
										l.Id == licenseeId && l.Status == LicenseeStatus.Terminated))
				throw new BusinessLogicValidationException(Errors.LicenseeUseStateError);

			var vehicleEntity = await _dbContext.Vehicles
							.Include(v => v.Licensee)
							.FirstOrDefaultAsync(v => v.AuthorityId == _userContext.AuthorityId && v.LicenseeId == licenseeId)
							?? throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);
			var oldEntity = GetVehicleEntityToSerialize(vehicleEntity);


			if (vehicleEntity.LicensePlate != vehicleWrite.LicensePlate)
				await CheckEntityAlreadyExist(vehicleWrite.LicensePlate);

			if (vehicleWrite is VehicleVariation vehicleVariation)
			{
				
				var vehicleHistory = vehicleEntity.Map(vehicleVariation.Note, DateTime.UtcNow);

				await _dbContext.AddAsync(vehicleHistory);

				vehicleEntity.SysStartTime = DateTime.UtcNow;
			}

			vehicleEntity.CarFuelType = vehicleWrite.CarFuelType.Value;
			vehicleEntity.InUseSince = vehicleWrite.InUseSince.Value;
			vehicleEntity.LicensePlate = vehicleWrite.LicensePlate;
			vehicleEntity.Model = vehicleWrite.Model;
			vehicleEntity.RegistrationDate = vehicleWrite.RegistrationDate.Value;
			_dbContext.Vehicles.Update(vehicleEntity);
			await _dbContext.SaveChangesAsync();
			await _systemEventManager.SendAsync(ApplicationsTopics.TaxiDriverEvents, new SystemEventBase
			{
				Payload = new TaxiDriverAuditEvent
				{
					AuthorityId = _userContext.AuthorityId,
					DisplayName = _userContext.DisplayName,
					TenantId = _userContext.TenantId,
					UserId = _userContext.SmartPAUserId!.Value,
					AuditPayload = new PayloadItem
					{
						Audit = new AuditWrite
						{
							ItemId = vehicleEntity.Id,
							ItemType = ItemTypes.Vehicle,
							MemoLine = vehicleWrite is VehicleVariation variation ? variation.Note : null,
							OperationType = vehicleWrite is VehicleVariation ? OperationTypes.Changing : OperationTypes.Updating
						},
						Data = oldEntity
					}
				}.JsonSerialize(false),
				Type = TaxiDriverEventTypes.TaxiDriverAudit
			});
		}

		async Task<FilterResult<T>> IVehiclesService.SearchVehicleVariations<T>(VehicleVariationFilterCriteria filterCriteria)
		{
			var fc = filterCriteria ?? new VehicleVariationFilterCriteria();

			if (string.IsNullOrWhiteSpace(fc.KeySelector))
			{
				fc.KeySelector = nameof(VehicleHistory.StartDate);
				fc.Ascending = false;
			}

			var query = _dbContext.LicenseesHistory
							.AsNoTracking()
							.Where(l => l.AuthorityId == _userContext.AuthorityId && l.VariationType == VariationTypes.VehiclesVariation);

			query = fc.VariationDateFrom.HasValue ?
			query.Where(q => q.SysStartTime >= fc.VariationDateFrom) : query;
			query = fc.CarFuelType.HasValue ? query.Where(q => q.VehicleCarFuelType.Equals(fc.CarFuelType)) : query;
			query = fc.VariationDateTo.HasValue ? query.Where(q => q.SysStartTime <= fc.VariationDateTo) : query;
			query = fc.Id.HasValue ? query.Where(q => q.VehicleId == fc.Id) : query;
			query = fc.Ids.Count > 0 ? query.Where(q => fc.Ids.Contains(q.VehicleId.GetValueOrDefault())) : query;
			query = fc.LicenseeId.HasValue ? query.Where(q => q.LicenseeId == fc.LicenseeId) : query;
			query = fc.LicenseeIds.Count > 0 ? query.Where(q => fc.LicenseeIds.Contains(q.LicenseeId)) : query;
			query = !string.IsNullOrWhiteSpace(fc.Note) ? query.Where(q => fc.Note.Contains(q.Note)) : query;

			var result = await query.OrderAndPageAsync(fc.ToTypedCriteria(fc.MapSortCriteria()));

			return result.MapFilterResult(m => m.Map<T>());
		}

		#region private methods

		async Task CheckEntityAlreadyExist(string plate)
		{
			if (await _dbContext.Vehicles.AsNoTracking()
						.AnyAsync(v => v.AuthorityId == _userContext.AuthorityId &&
										v.LicensePlate == plate))
				throw new BusinessLogicValidationException(Errors.VehicleAlredyExist, plate);
		}

		static VehicleEntity GetVehicleEntityToSerialize(VehicleEntity entity) => new()
		{
			AuthorityId = entity.AuthorityId,
			CarFuelType = entity.CarFuelType,
			Id = entity.Id,
			InUseSince = entity.InUseSince,
			Licensee = new LicenseeEntity
			{
				AuthorityId = entity.Licensee.AuthorityId,
				EndDate = entity.Licensee.EndDate,
				ExpireActivityCause = entity.Licensee.ExpireActivityCause,
				Id = entity.Licensee.Id,
				Number = entity.Licensee.Number,
				ReleaseDate = entity.Licensee.ReleaseDate,
				SysStartTime = entity.Licensee.SysStartTime,
				Status = entity.Licensee.Status,
				SubShiftId = entity.Licensee.SubShiftId,
				TaxiDriverAssociationId = entity.Licensee.TaxiDriverAssociationId,
				Type = entity.Licensee.Type
			},
			LicenseeId = entity.LicenseeId,
			LicensePlate = entity.LicensePlate,
			Model = entity.Model,
			RegistrationDate = entity.RegistrationDate,
			SysStartTime = entity.SysStartTime
		};

		#endregion
	}
}