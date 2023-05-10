using SmartTech.Common;
using SmartTech.Common.ApplicationUsers;

namespace Asf.Taxi.BusinessLogic
{
	public class TaxiPermissions : BasePermissions, IUserPermissions
	{
		public TaxiPermissions()
		{

		}

		public const string Taxi_Admin = nameof(Taxi_Admin);

		public const string Taxi_Operator = nameof(Taxi_Operator);

		public const string Taxi_Driver = nameof(Taxi_Driver);

		public const string Taxi_Executive = nameof(Taxi_Executive);

		private TaxiPermissions(string roleCode) : base(roleCode)
		{

		}

		public static TaxiPermissions Administrator => new(Taxi_Admin);

		public static TaxiPermissions Operator => new(Taxi_Operator);

		public static TaxiPermissions TaxiDriver => new(Taxi_Driver);

		public static TaxiPermissions TaxiExecutive => new(Taxi_Executive);

		IEnumerable<string> IUserPermissions.ValidPermissions => new List<string>()
		{
			Administrator.Code,
			Operator.Code,
			TaxiDriver.Code,
			TaxiExecutive.Code
		};
	}
}