using Asf.Taxi.BusinessLogic;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using SmartTech.Common.API;
using SmartTech.Common.Enums;
using SmartTech.Common.Models;
using SmartTech.Common.Web.Security;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.API.Controllers
{
    public class DigitalSignController : APIControllerBase
    {
        readonly IDigitalSignService _digitalSignService;

        public DigitalSignController(IDigitalSignService digitalSignService) => _digitalSignService = digitalSignService;

        [HttpGet("credential")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<DigitalSignCredential?> Credential() =>
            await _digitalSignService.GetCredential();

        [HttpGet("credentials")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Admin)]
        public async Task<FilterResult<CredentialUser>> SearchCredentials([FromQuery] CredentialUserFilterCriteria search) =>
            await _digitalSignService.SearchCredentials(search);

        [HttpPut("credentials")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task SaveCredentials([FromBody] DigitalSignCredential credential) =>
            await _digitalSignService.UpsertCredential(credential);

        [HttpDelete("credentials")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task DeleteCredential() =>
            await _digitalSignService.DeleteCredential();

        [HttpPost("otp")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task SendOTP([FromBody] DigitalSignCredential userCredential) =>
            await _digitalSignService.SendOTP(userCredential);

        [HttpPost("requestsregisters/sign/{signType}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<IEnumerable<ErrorModel>> MultipleSign(SignTypes signType, [FromBody] MultipleDigitalSignWrite multipleDigitalSign) =>
            await _digitalSignService.SignMultipleDocuments(signType, multipleDigitalSign);

        [HttpGet("requestsregisters/{requestRegisterId}/signErrors")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<string> GetSignError(long requestRegisterId) =>
            await _digitalSignService.GetDigitalSignError(requestRegisterId);
    }
}