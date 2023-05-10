using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.Templates
{
    public class TemplateBase
    {
        [Required]
        [Label(ResourcesConst.Description)]
        public string? Description { get; set; }
    }
}