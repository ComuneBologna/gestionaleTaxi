using Asf.Taxi.BusinessLogic;
using Asf.Taxi.BusinessLogic.Models.Templates;
using Asf.Taxi.BusinessLogic.Models.Requests;
using Asf.Taxi.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using SmartTech.Common.API;
using SmartTech.Common.Web.Security;
using SmartTech.Infrastructure.Search;
using SmartTech.Infrastructure.Attachments;
using Asf.Taxi.API.Models;
using Asf.Taxi.API.Mappers;

namespace Asf.Taxi.API.Controllers
{
    public class TemplatesController : APIControllerBase
    {
        readonly ITemplateService _templateService;

        public TemplatesController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        #region CRUD Templates
        [HttpGet()]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<FilterResult<TemplateDetails>> GetTemplates([FromQuery] TemplatesFilterCriteria criteria) =>
                                        await _templateService.SearchTemplates<TemplateDetails>(criteria);

        [HttpGet("all")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<List<TemplateInfo>> GetTemplatesAsList() =>
            (await _templateService.SearchTemplates<TemplateInfo>(new TemplatesFilterCriteria
            {
                ItemsPerPage = int.MaxValue,
                PageNumber = 1,
                Ascending = true
            })).Items.ToList();

        [HttpGet("{templateId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<TemplateDetails?> GetTemplateById(long templateId) =>
            (await _templateService.SearchTemplates<TemplateDetails>(new TemplatesFilterCriteria
            {
                Id = templateId
            })).Items.FirstOrDefault();

        [HttpPost]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<long> PostTemplate([FromBody] TemplateWrite template) =>
            await _templateService.AddTemplate(template);

        [HttpPost("upload")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<AttachmentInfo> UploadFile([FromForm] IFormFile file)
        {
            var attachment = (file ?? (await Request.ReadFormAsync()).Files[0]).Map();

            return await _templateService.UploadTemplate(attachment);
        }

        [HttpGet("{templateId}/download")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<string?> GetTemplate(long templateId) =>
                                    await _templateService.DownloadTemplate(templateId);

        [HttpDelete("{templateId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task DeleteTemplate(long templateId) =>
                                    await _templateService.DeleteTemplate(templateId);

        [HttpGet("tags")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<List<Tag>> GetTags() => await _templateService.RetrieveTags();
        #endregion

        [HttpPost("{templateId}/licensees/{licenseeId}/upload")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<long> PostRequestFromFile(long licenseeId, long templateId, [FromForm] IFormFile file)
        {
            var attachment = (file ?? (await Request.ReadFormAsync()).Files[0]).Map();

            return await _templateService.UploadRequest(licenseeId, templateId, new AdditionalInformationWrite()
            {
                Attachment = attachment
            });
        }

        [HttpPost("{templateId}/licensees/{licenseeId}/draft")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<string> GetDraftRequest(long licenseeId, long templateId, [FromBody] AdditionalInformation additionalInformation) =>
                                    await _templateService.CreateDraftDocument(licenseeId, templateId, additionalInformation);

        [HttpGet("{templateId}/licensees/{licenseeId}/preview")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<string?> GetPreview(long licenseeId, long templateId) =>
                                    await _templateService.GetImagePreview(licenseeId, templateId);

        [HttpGet("requestsregisters/{requestRegisterId}/download")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<FileContentResult> DowloadRequest(long requestRegisterId)
        {
            var result = await _templateService.DownloadRequestDocument(requestRegisterId);
            return File(result.Content!, result.MimeType!, result.FileName);
        }

        [HttpPut("requestsregisters/{requestRegisterId}/rollback")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task RequestStatusRollback(long requestRegisterId) =>
            await _templateService.RegistersRollback(requestRegisterId);

        [HttpGet("requestsregisters")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<FilterResult<RequestRegister>> GetRequests([FromQuery] RequestsRegisterFilterCriteria criteria) =>
            await _templateService.SearchRegisters<RequestRegister>(criteria);

        [HttpGet("requestsregisters/tosign")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<FilterResult<RequestRegisterInfo>> GetRequestsToSign([FromQuery] RequestsRegisterFilterCriteriaAPI criteria) =>
            await _templateService.SearchRegisters<RequestRegisterInfo>(criteria.Map());
    }
}