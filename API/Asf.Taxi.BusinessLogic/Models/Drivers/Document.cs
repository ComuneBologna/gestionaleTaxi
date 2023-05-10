using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
    public class Document
    {
        public long? Id { get; set; }

        [Required]
        [Label(ResourcesConst.DocumentType)]
        public DocumentTypes? Type { get; set; }

        [Required]
        [Label(ResourcesConst.Number)]
        public string? Number { get; set; }

        [Required]
        [Label(ResourcesConst.ReleasedBy)]
        public string? ReleasedBy { get; set; }

        [Required]
        [Label(ResourcesConst.ValidityDate)]
        public DateTime? ValidityDate { get; set; }
    }
}