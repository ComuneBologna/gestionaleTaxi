using Asf.Taxi.BusinessLogic.Models;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Services
{
	public interface IShiftsService
	{
		Task<FilterResult<T>> SearchShifts<T>(ShiftsSearchCriteria criteria) where T : ShiftBase;

		Task<long> AddShift(ShiftWrite shift);

		Task EditShift(long shiftId, ShiftWrite shift);

		Task DeleteShift(long shiftId);
	}
}