using SmartTech.Common.ApplicationUsers.Entities;

namespace Asf.Taxi.DAL.Entities
{
    public class TaxiAuthorityUserEntity : AuthorityUserEntity
    {
        public long? DriverId { get; set; }
    }
}