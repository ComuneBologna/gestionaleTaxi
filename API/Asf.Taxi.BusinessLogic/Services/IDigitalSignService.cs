using Asf.Taxi.BusinessLogic.Models;
using SmartTech.Common.Enums;
using SmartTech.Common.Models;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Services
{
    public interface IDigitalSignService
    {
        Task<FilterResult<CredentialUser>> SearchCredentials(CredentialUserFilterCriteria search);

        Task<DigitalSignCredential?> GetCredential();

        Task UpsertCredential(DigitalSignCredential credential);

        Task DeleteCredential();

        Task<IEnumerable<ErrorModel>> SignMultipleDocuments(SignTypes signType, MultipleDigitalSignWrite multipleDigitalSign);

        Task SendOTP(DigitalSignCredential userCredential);

        Task<string> GetDigitalSignError(long requestRegisterId);
    }
}