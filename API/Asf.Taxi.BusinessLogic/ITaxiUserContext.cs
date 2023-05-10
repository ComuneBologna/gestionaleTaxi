using SmartTech.Common.Authentication;

namespace Asf.Taxi.BusinessLogic
{
	public interface ITaxiUserContext : IUserContext
	{
		public long? DriverId { get; }

		public bool IsExecutive { get; }
	}
}