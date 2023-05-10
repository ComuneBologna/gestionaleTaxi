using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Infrastructure.Attachments;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models.Requests
{
    public class AdditionalInformationWrite: AdditionalInformation
    {
        [Required]
        [Label(ResourcesConst.File)]
        public Attachment? Attachment { get; set; }
    }
}