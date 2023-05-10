using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.Templates
{
    public class TemplateWrite: TemplateBase
    {
        [Required]
        [Label(ResourcesConst.File)]
        public string? FileId { get; set; }
    }
}