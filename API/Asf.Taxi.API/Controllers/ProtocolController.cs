using Asf.Taxi.API.Models;
using Asf.Taxi.BusinessLogic;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Models.ProtocolEmails;
using Asf.Taxi.BusinessLogic.Models.Requests;
using Asf.Taxi.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using SmartTech.Common.API;
using SmartTech.Common.Enums;
using SmartTech.Common.Models;
using SmartTech.Common.Web.Security;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.API.Controllers
{
    public class ProtocolController : APIControllerBase
    {
        readonly IProtocolService _protocolService;

        public ProtocolController(IProtocolService protocolService)
        {
            _protocolService = protocolService;
        }

        [HttpPost("requestsregisters/{requestRegisterId}/protocols")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<ProtocolDataOutput> AddProtocol(long requestRegisterId, [FromBody] ProtocolInput protocol) =>
            await _protocolService.ProtocolRequestDocument(requestRegisterId, protocol);

        [HttpPost("requestsregisters/{requestRegisterId}/protocols/send")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task SendProtocolEmail(long requestRegisterId, [FromBody] ProtocolSendMailInput protocolMail) =>
            await _protocolService.SendProtocolEmail(requestRegisterId, protocolMail);

        [HttpGet("requestsregisters/{requestRegisterId}/checkemailstatus")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<EmailStatus> GetProtocolEmailStatus(long requestRegisterId) =>
            await _protocolService.GetProtocolEmailStatus(requestRegisterId);

        [HttpPost("requestsregisters/{requestRegisterId}/protocols/startprocess/{processTypeCode}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task StartProcess(long requestRegisterId, long processTypeCode) =>
            await _protocolService.StartProcess(requestRegisterId, processTypeCode);

        [HttpPost("requestsregisters/{requestRegisterId}/protocols/stopprocess/{processTypeCode}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task StopProcess(long requestRegisterId, long processTypeCode) =>
            await _protocolService.StopProcess(requestRegisterId, processTypeCode);

        [HttpPost("requestsregisters/{requestRegisterId}/protocols/releaseinchargeofthefolder")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task ReleaseInChargeOfTheFolder(long requestRegisterId) =>
            await _protocolService.ReleaseInChargeOfTheFolder(requestRegisterId);

        [HttpGet("external/documents/search")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<FilterResult<ProxyDocument>> SearchExternalDocuments([FromQuery] ProxyDocumentSearchCriteriaModel searchCriteria)
            => await _protocolService.SearchExternalDocuments(new()
            {
                Ascending = searchCriteria.Ascending,
                ItemsPerPage = searchCriteria.ItemsPerPage,
                KeySelector = searchCriteria.KeySelector,
                PageNumber = searchCriteria.PageNumber,
                ProtocolNumber = searchCriteria.ProtocolNumber,
                ProtocolYear = searchCriteria.ProtocolYear,
                Status = searchCriteria.Status,
                Title = searchCriteria.Title,
                Type = searchCriteria.Type
            });

        [HttpGet("external/documents/{externalProxyDocumentId}/attachments")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<IEnumerable<ProxyExternalDocumentAttachment>> GetExternalAttachmentsByExternalId(string externalProxyDocumentId)
            => await _protocolService.GetExternalAttachmentsByExternalId(externalProxyDocumentId);

        [HttpPost("requestsregisters/{requestRegisterId}/leaddocuments/group")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task GroupSmartPADocumentToLeadDocument(long requestRegisterId, [FromBody] LeadProtocolDataInput leadDocument) =>
            await _protocolService.GroupSmartPADocumentToLeadDocument(requestRegisterId, leadDocument);

        [HttpGet("requestsregisters/{requestRegisterId}/leaddocuments")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<LeadProtocolDataInput> GetLeadDocument(long requestRegisterId) => await _protocolService.GetLeadDocument(requestRegisterId);

        [HttpGet("processCodes/autocomplete")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<IEnumerable<ProcessCode>> ProcessCodesAutocomplete([FromQuery] string fullTextSearch)
            => await _protocolService.GetProcessCodes(fullTextSearch);

        #region ProtocolEmails
        [HttpGet("emails")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<FilterResult<ProtocolEmail>> GetProtocolEmails([FromQuery] ProtocolEmailSearchCriteria criteria)
            => await _protocolService.SearchProtocolEmails<ProtocolEmail>(criteria);

        [HttpGet("emails/{emailId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<ProtocolEmail?> GetProtocolEmail(long emailId)
            => (await _protocolService.SearchProtocolEmails<ProtocolEmail>(new ProtocolEmailSearchCriteria { Id = emailId })).Items.FirstOrDefault();

        [HttpGet("emails/list")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<IEnumerable<ProtocolEmailInfo>> GetProtocolEmailsList()
            => (await _protocolService.SearchProtocolEmails<ProtocolEmailInfo>(new ProtocolEmailSearchCriteria
            {
                Active = true,
                ItemsPerPage = int.MaxValue,
                KeySelector = nameof(ProtocolEmail.Email),
                Ascending = true
            })).Items;

        [HttpPost("emails")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<long> PostProtocolEmail([FromBody] ProtocolEmailWrite protocolEmail)
            => await _protocolService.AddProtocolEmail(protocolEmail);

        [HttpPut("emails/{emailId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task PutProtocolEmail(long emailId, [FromBody] ProtocolEmailUpdate protocolEmail)
            => await _protocolService.UpdateProtocolEmail(emailId, protocolEmail);

        [HttpDelete("emails/{emailId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task DeleteProtocolEmail(long emailId)
            => await _protocolService.DeleteProtocolEmail(emailId);
        #endregion
    }
}