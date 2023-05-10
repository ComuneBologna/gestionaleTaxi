using SmartTech.Infrastructure.Attachments;
using Asf.Taxi.BusinessLogic.Models.Templates;
using Asf.Taxi.BusinessLogic.Models.Requests;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.BusinessLogic.Services
{
    public interface ITemplateService
    {
        #region Templates

        Task<FilterResult<T>> SearchTemplates<T>(TemplatesFilterCriteria criteria) where T : TemplateBase;

        Task<long> AddTemplate(TemplateWrite template);

        Task<AttachmentInfo> UploadTemplate(Attachment template);

        Task<string?> DownloadTemplate(long templateId);

        Task DeleteTemplate(long templateId);

        Task<List<Tag>> RetrieveTags();

        #endregion

        #region Requests

        Task RegistersRollback(long requestRegisterId);

        Task<string> CreateDraftDocument(long licenseeId, long templateId, AdditionalInformation information);

        //Task<long> SaveRequest(long licenseeId, long templateId, AdditionalInformation information);

        Task<FilterResult<T>> SearchRegisters<T>(RequestsRegisterFilterCriteria criteria) where T : RequestRegisterBase;

        Task<DMSFileResult> DownloadRequestDocument(long requestRegisterId);

        Task<long> UploadRequest(long licenseeId, long templateId, AdditionalInformationWrite attachment);

        Task<string?> GetImagePreview(long licenseeId, long templateId);

        Task SendRequests(long licenseeId, RequestRegisterSend requestsToSend);

        Task UpdateRequest(long licenseeId, long requestRegisterId);

        Task DeleteRequest(long licenseeId, long requestRegisterId);

        #endregion
    }
}