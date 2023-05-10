using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.Licensees
{
    public class LicenseeNCCWrite : LicenseeWrite
    {
        [Required]
        [Label(ResourcesConst.GarageAddress)]
        public string? GarageAddress { get; set; }

        [Required]
        [Label(ResourcesConst.IsFinancialAdministration)]
        public bool? IsFinancialAdministration { get; set; }
    }
}