using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.DAL.Entities;
using SmartTech.Infrastructure.Extensions;
using System.Linq.Expressions;

namespace Asf.Taxi.BusinessLogic.Mapper
{
	static class ShiftMapper
	{
		public static Expression<Func<ShiftEntity, dynamic>> MapSortCriteria<T>(this ShiftsSearchCriteria ssc) where T : ShiftBase =>
			typeof(T).Equals(typeof(Shift)) ?
			ssc.KeySelector.FirstCharToUpper() switch
			{
				nameof(Shift.BreakInHours) => x => x.BreakInHours,
				nameof(Shift.BreakThresholdActivationInHour) => x => x.BreakThresholdActivationInHour,
				nameof(Shift.DurationInHour) => x => x.DurationInHour,
				nameof(Shift.HandicapAfterInHour) => x => x.HandicapAfterInHour,
				nameof(Shift.HandicapBeforeInHour) => x => x.HandicapBeforeInHour,
				nameof(Shift.IsHandicapMode) => x => x.IsHandicapMode,
				nameof(Shift.Name) => x => x.Name,
				nameof(Shift.RestDayFrequencyInDays) => x => x.RestDayFrequencyInDays,
				_ => x => x.Id
			} : ssc.KeySelector.FirstCharToUpper() switch
			{
				nameof(ShiftBase.DurationInHour) => x => x.DurationInHour,
				nameof(ShiftBase.Name) => x => x.Name,
				_ => x => x.Id
			};

		public static T Map<T>(this ShiftEntity shift) where T : ShiftBase =>
			typeof(T).Equals(typeof(Shift)) ?
			new Shift
			{
				BreakInHours = shift.BreakInHours,
				BreakThresholdActivationInHour = shift.BreakThresholdActivationInHour,
				DurationInHour = shift.DurationInHour,
				HandicapAfterInHour = shift.HandicapAfterInHour,
				HandicapBeforeInHour = shift.HandicapBeforeInHour,
				Id = shift.Id,
				IsHandicapMode = shift.IsHandicapMode,
				Name = shift.Name,
				RestDayFrequencyInDays = shift.RestDayFrequencyInDays,
				SubShifts = shift.SubShifts.Select(ss => new SubShift
				{
					Id = ss.Id,
					Name = ss.Name,
					RestDay = ss.RestDay
				}).ToList()
			} as T :
			typeof(T) == typeof(ShiftInfo) ?
			new ShiftInfo
			{
				DurationInHour = shift.DurationInHour,
				Id = shift.Id,
				Name = shift.Name
			} as T :
			new ShiftBase
			{
				DurationInHour = shift.DurationInHour,
				Name = shift.Name
			} as T;

		public static ShiftEntity Map(this ShiftWrite shiftWrite, long authorityId) =>
				new()
				{
					AuthorityId = authorityId,
					BreakInHours = shiftWrite.BreakInHours.Value,
					BreakThresholdActivationInHour = shiftWrite.BreakThresholdActivationInHour.Value,
					DurationInHour = shiftWrite.DurationInHour.Value,
					HandicapAfterInHour = shiftWrite.HandicapAfterInHour,
					HandicapBeforeInHour = shiftWrite.HandicapBeforeInHour,
					IsEnabled = true,
					IsHandicapMode = shiftWrite.IsHandicapMode.Value,
					Name = shiftWrite.Name,
					RestDayFrequencyInDays = shiftWrite.RestDayFrequencyInDays.Value,
					SubShifts = shiftWrite.SubShifts.Select(ss => new SubShiftEntity
					{
						AuthorityId = authorityId,
						IsEnabled = true,
						Name = ss.Name,
						RestDay = ss.RestDay
					}).ToList()
				};

		public static bool SubShiftIsModified(this SubShiftEntity subShift, SubShift subShiftToCompare) =>
			subShift.Name != subShiftToCompare.Name || subShift.RestDay != subShiftToCompare.RestDay;
	}
}