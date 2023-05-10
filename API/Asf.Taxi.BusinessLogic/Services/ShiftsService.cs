using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.BusinessLogic.Mapper;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.DAL;
using Asf.Taxi.DAL.Entities;
using Asf.Taxi.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using SmartTech.Infrastructure;
using SmartTech.Infrastructure.DataAccessLayer.EFCore;
using SmartTech.Infrastructure.Exceptions;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Search;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Services
{
	class ShiftsService : IShiftsService
	{
		readonly ITaxiUserContext _userContext;
		readonly TaxiDriverDBContext _dbContext;

		public ShiftsService(ITaxiUserContext userContext, TaxiDriverDBContext dbContext)
		{
			_dbContext = dbContext;
			_userContext = userContext;
		}

		async Task<long> IShiftsService.AddShift(ShiftWrite shift)
		{
			shift?.Validate();

			var shiftEntity = shift.Map(_userContext.AuthorityId);

			await _dbContext.Shifts.AddAsync(shiftEntity);
			await _dbContext.SaveChangesAsync();

			return shiftEntity.Id;
		}

		async Task IShiftsService.DeleteShift(long shiftId)
		{
			if (shiftId <= 0)
				throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);

			var shift = await _dbContext.Shifts.FirstOrDefaultAsync(s => s.AuthorityId == _userContext.AuthorityId && s.Id == shiftId && s.IsEnabled)
				?? throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);

			if (await _dbContext.Licensees.AsNoTracking()
						.AnyAsync(l => l.AuthorityId == _userContext.AuthorityId && l.SubShift.ShiftId == shiftId && l.Status != LicenseeStatus.Terminated))
				throw new BusinessLogicValidationException(Errors.ShiftInUse);

			shift.IsEnabled = false;
			_dbContext.Shifts.Update(shift);
			await _dbContext.SaveChangesAsync();
		}

		async Task IShiftsService.EditShift(long shiftId, ShiftWrite shift)
		{
			if (shiftId <= 0)
				throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);

			shift?.Validate();

			var shiftEntity = await _dbContext.Shifts
							.Include(s => s.SubShifts)
							.FirstOrDefaultAsync(s => s.AuthorityId == _userContext.AuthorityId && s.Id == shiftId && s.IsEnabled)
							?? throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);

			shiftEntity.BreakInHours = shift.BreakInHours.Value;
			shiftEntity.BreakThresholdActivationInHour = shift.BreakThresholdActivationInHour.Value;
			shiftEntity.DurationInHour = shift.DurationInHour.Value;
			shiftEntity.HandicapAfterInHour = shift.HandicapAfterInHour;
			shiftEntity.HandicapBeforeInHour = shift.HandicapBeforeInHour;
			shiftEntity.IsHandicapMode = shift.IsHandicapMode.Value;
			shiftEntity.Name = shift.Name;
			shiftEntity.RestDayFrequencyInDays = shift.RestDayFrequencyInDays.Value;
			shiftEntity.SubShifts.Except(shift.SubShifts.Where(ss => ss.Id > 0).Select(ss =>
					new SubShiftEntity
					{
						Id = ss.Id,
						AuthorityId = shiftEntity.AuthorityId,
						Name = ss.Name,
						RestDay = ss.RestDay,
						ShiftId = shiftEntity.Id
					}
			), GenericComparer.Create((SubShiftEntity ss) => ss.Id))
				.ForEach(ss => shiftEntity.SubShifts.Remove(ss));
			shiftEntity.SubShifts.Join(shift.SubShifts, db => db.Id, e => e.Id, (db, e) => new
			{
				db,
				e
			}).Where(w => w.db.SubShiftIsModified(w.e))
			.ForEach(w =>
			{
				w.db.Name = w.e.Name;
				w.db.RestDay = w.e.RestDay;
			});
			shift.SubShifts.Where(ss => ss.Id <= 0)
				.ForEach(ss => shiftEntity.SubShifts.Add(new SubShiftEntity
				{
					AuthorityId = shiftEntity.AuthorityId,
					IsEnabled = true,
					Name = ss.Name,
					RestDay = ss.RestDay,
					ShiftId = shiftEntity.Id,
				}));
			_dbContext.Shifts.Update(shiftEntity);
			await _dbContext.SaveChangesAsync();
		}

		async Task<FilterResult<T>> IShiftsService.SearchShifts<T>(ShiftsSearchCriteria criteria)
		{
			var sc = criteria ?? new()
			{
				ItemsPerPage = 50,
				PageNumber = 1
			};
			var query = _dbContext.Shifts
									.Include(s => s.SubShifts)
									.AsNoTracking()
									.Where(s => s.AuthorityId == _userContext.AuthorityId && s.IsEnabled);

			query = sc.Id.HasValue ? query.Where(q => q.Id == sc.Id) : query;
			query = sc.DurationInHour.HasValue ? query.Where(q => q.DurationInHour == sc.DurationInHour) : query;
			query = !string.IsNullOrWhiteSpace(sc.Name) ? query.Where(q => q.Name.Contains(sc.Name)) : query;

			var result = await query.OrderAndPageAsync(sc.ToTypedCriteria(sc.MapSortCriteria<T>()));

			return result.MapFilterResult(m => m.Map<T>());
		}
	}
}