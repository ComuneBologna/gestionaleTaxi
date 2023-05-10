using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Models.ProtocolEmails;
using Asf.Taxi.BusinessLogic.Models.Requests;
using SmartTech.Common.Enums;
using SmartTech.Common.Models;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Services
{
    public interface IProtocolService
    {
        Task<ProtocolDataOutput> ProtocolRequestDocument(long requestRegisterId, ProtocolInput protocol);

        Task SendProtocolEmail(long requestRegisterId, ProtocolSendMailInput protocolMail);

        Task<EmailStatus> GetProtocolEmailStatus(long requestRegisterId);

        Task StartProcess(long requestRegisterId, long processTypeCode);

        Task StopProcess(long requestRegisterId, long processTypeCode);

        Task ReleaseInChargeOfTheFolder(long requestRegisterId);

        Task<FilterResult<ProxyDocument>> SearchExternalDocuments(ProxyDocumentSearchCriteria searchCriteria);

        Task<IEnumerable<ProxyExternalDocumentAttachment>> GetExternalAttachmentsByExternalId(string externalDocumentId);

        Task GroupSmartPADocumentToLeadDocument(long requestRegisterId, LeadProtocolDataInput leadDocument);

        Task<LeadProtocolDataInput> GetLeadDocument(long requestRegisterId);

        Task<IEnumerable<ProcessCode>> GetProcessCodes(string? fullTextSearch = null);

        #region ProtocolEmails
        Task<long> AddProtocolEmail(ProtocolEmailWrite protocolEmailWrite);

        Task UpdateProtocolEmail(long id, ProtocolEmailUpdate protocolEmailUpdate);

        Task DeleteProtocolEmail(long id);

        Task<FilterResult<T>> SearchProtocolEmails<T>(ProtocolEmailSearchCriteria criteria) where T : ProtocolEmailBase;
        #endregion
    }
}