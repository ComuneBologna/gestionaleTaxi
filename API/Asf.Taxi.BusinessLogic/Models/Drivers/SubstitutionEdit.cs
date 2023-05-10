using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
    public class SubstitutionEdit : SubstitutionWrite
    {
        [Required]
        [Label(ResourcesConst.RequestStatus)]
        public SubstitutionStatus? Status { get; set; }
    }
}
