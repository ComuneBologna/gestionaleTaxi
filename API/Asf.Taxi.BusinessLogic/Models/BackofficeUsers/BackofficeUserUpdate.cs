using SmartTech.Common.ApplicationUsers.Models;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
    public class BackofficeUserUpdate : UserUpdate
    {
        [RequiredIf(nameof(IsTaxiDriver))]
        public long? DriverId { get; set; }

        public bool IsTaxiDriver => PermissionCode == TaxiPermissions.TaxiDriver.Code;
    }
}