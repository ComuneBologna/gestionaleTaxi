using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
    public class PhysicalPersonWrite : PersonWriteBase
    {
        [Required]
        [Label(ResourcesConst.FirstName)]
        public string? FirstName { get; set; }

        [Required]
        [Label(ResourcesConst.LastName)]
        public string? LastName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]{6}\d\d[a-zA-Z]\d\d[a-zA-Z]\d\d\d[a-zA-Z]$", ResourcesConst.FiscalCodeFormat)]
        [Label(ResourcesConst.FiscalCode)]
        public string? FiscalCode { get; set; }

        [Required]
        [Label(ResourcesConst.BirthDate)]
        public DateTime? BirthDate { get; set; }

        [Label(ResourcesConst.BirthPlace)]
        public string? BirthCity { get; set; }

        public string? BirthProvince { get; set; }

        [RegularExpression(@"^\d{5}$", ResourcesConst.ZipCodeFormat)]
        public string? ZipCode { get; set; }

        public string? ResidentCity { get; set; }

        public string? ResidentProvince { get; set; }
    }
}
