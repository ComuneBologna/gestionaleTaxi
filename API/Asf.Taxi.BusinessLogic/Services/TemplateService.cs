using Asf.Taxi.DAL;
using SmartTech.Infrastructure;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Asf.Taxi.DAL.Enums;
using Asf.Taxi.DAL.Entities;
using SmartTech.Infrastructure.Storage;
using SmartTech.Common;
using Asf.Taxi.BusinessLogic.Models.Templates;
using Asf.Taxi.BusinessLogic.Models.Requests;
using SmartTech.Infrastructure.DataAccessLayer.EFCore;
using SmartTech.Infrastructure.Extensions;
using Asf.Taxi.BusinessLogic.Mapper;
using SmartTech.Infrastructure.Search;
using SmartTech.Infrastructure.Exceptions;
using SmartTech.Infrastructure.Validations;
using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.BusinessLogic.Extensions;
using SmartTech.Common.Models;
using SmartTech.Common.Services;
using Microsoft.Extensions.Options;
using SmartTech.Common.Extensions;
using SmartTech.Infrastructure.SystemEvents;
using Asf.Taxi.BusinessLogic.Events;
using SmartTech.Infrastructure.Attachments;

namespace Asf.Taxi.BusinessLogic.Services
{
    public class TemplateService : ITemplateService
    {
        readonly IOrganizationalChart _organizationalChartService;
        readonly ITaxiUserContext _userContext;
        readonly ILogger<TemplateService> _logger;
        readonly IFileStorage _fileStorage;
        readonly IDocumentManagementSystemService _dmsService;
        readonly IHttpClientFactory _httpClientFactory;
        readonly ISystemEventManager _sysEventManager;
        readonly SmartPAClientData _smartPAClientData;
        readonly TaxiDriverDBContext _dbContext;
        readonly List<string> _validMimeTypes = new()
        {
            MimeTypes.MicrosoftWord.Code,
            MimeTypes.MicrosoftOfficeOOXMLWordDocument.Code
        };

        public TemplateService(IFileStorage fileStorage, TaxiDriverDBContext dbContext, ILogger<TemplateService> logger, ITaxiUserContext userContext,
            IDocumentManagementSystemService dmsService, IHttpClientFactory httpClientFactory, IOptions<SmartPAClientData> smartPaClientData,
            ISystemEventManager sysEventManager, IOrganizationalChart organizationalChartService)
        {
            _logger = logger;
            _userContext = userContext;
            _dbContext = dbContext;
            _fileStorage = fileStorage;
            _dmsService = dmsService;
            _httpClientFactory = httpClientFactory;
            _smartPAClientData = smartPaClientData.Value;
            _sysEventManager = sysEventManager;
            _organizationalChartService = organizationalChartService;
        }

        #region Templates
        async Task<FilterResult<T>> ITemplateService.SearchTemplates<T>(TemplatesFilterCriteria criteria)
        {
            var fc = criteria ?? new();
            var query = _dbContext.Templates.AsNoTracking()
                            .Where(t => t.AuthorityId == _userContext.AuthorityId && !t.Deleted);

            query = fc.Id.HasValue ? query.Where(q => q.Id == fc.Id) : query;
            query = !string.IsNullOrWhiteSpace(fc.Description) ? query.Where(q => q.Description!.Contains(fc.Description)) : query;
            query = !string.IsNullOrWhiteSpace(fc.FileName) ? query.Where(q => q.FileName!.Contains(fc.FileName)) : query;
            query = (fc.Ids?.Any() ?? false) ? query.Where(q => fc.Ids.Contains(q.Id)) : query;

            var result = await query.OrderAndPageAsync(fc.ToTypedCriteria(fc.MapSortCriteria()));
            return result.MapFilterResult(m => m.MapGetTemplate<T>());
        }

        async Task<long> ITemplateService.AddTemplate(TemplateWrite template)
        {
            template.Validate();

            var obj = template.MapPostTemplate(_userContext.AuthorityId);

            var path = $"{_userContext.AuthorityId}/templates/{template.FileId}";

            var data = await _fileStorage.GetFileInfoAsync(ApplicationsBlobStorages.Taxi, path);
            if (data == default)
                throw new BusinessLogicValidationException(Errors.AttachmentContentIsMandatory);

            obj.FileName = data.FileName;
            obj.MimeType = data.MimeType;

            await _dbContext.Templates.AddAsync(obj);
            await _dbContext.SaveChangesAsync();
            return obj.Id;
        }

        async Task ITemplateService.DeleteTemplate(long templateId)
        {
            var template = await _dbContext.Templates.AsNoTracking().FirstOrDefaultAsync(a => a.Id == templateId && a.AuthorityId == _userContext.AuthorityId && !a.Deleted);

            if (template == default || string.IsNullOrWhiteSpace(template.FileId))
                throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound, Errors.Template_TemplateNotFound);

            var path = $"{_userContext.AuthorityId}/templates/{template.FileId}";

            await _fileStorage.DeleteFileAsync(ApplicationsBlobStorages.Taxi, path);
            template.Deleted = true;
            template.LastUpdate = DateTime.UtcNow;

            _dbContext.Templates.Update(template);
            await _dbContext.SaveChangesAsync();
        }

        async Task<List<Tag>> ITemplateService.RetrieveTags()
        {
            await Task.CompletedTask;
            List<Tag> tags = new();

            foreach (var tag in TagsConstants.Tags)
            {
                tags.Add(new Tag
                {
                    Description = tag.Key,
                    Value = tag.Value
                });
            }
            return tags;
        }

        #region TemplateAttachments

        async Task<AttachmentInfo> ITemplateService.UploadTemplate(Attachment template)
        {
            template.Validate();

            var mimeType = template.MimeType ?? MimeTypes.GetMimeType(template.FileName!);

            if (string.IsNullOrWhiteSpace(mimeType) || !_validMimeTypes.Contains(mimeType))
                throw new BusinessLogicValidationException(string.Format(Errors.InvalidAttachmentExtension, mimeType));

            if ((template.Content?.Length ?? 0) <= 0)
                throw new BusinessLogicValidationException(Errors.AttachmentContentIsMandatory);

            var id = Guid.NewGuid().ToString();
            var path = $"{_userContext.AuthorityId}/templates/{id}";
            var attachmentInfo = new AttachmentInfo
            {
                FileName = template.FileName,
                Id = id,
                MimeType = mimeType,
            };

            await _fileStorage.UploadFileAsync(ApplicationsBlobStorages.Taxi, path, template.Content!, attachmentInfo.MimeType, attachmentInfo.FileName!);

            return attachmentInfo;
        }

        async Task<string?> ITemplateService.DownloadTemplate(long templateId)
        {
            var template = await _dbContext.Templates.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == templateId &&
                                            a.AuthorityId == _userContext.AuthorityId && !a.Deleted)
                ?? throw new BusinessLogicValidationException(Errors.Template_TemplateNotFound);

            if (string.IsNullOrWhiteSpace(template.FileId))
                return default;

            var path = $"{_userContext.AuthorityId}/templates/{template.FileId}";
            return (await _fileStorage.DownloadFileAsync(ApplicationsBlobStorages.Taxi, path, leaseTimeInMinutes: 5)).Data;
        }

        #endregion

        #endregion

        #region Requests

        // Annulla una richiesta di firma
        async Task ITemplateService.RegistersRollback(long requestRegisterId)
        {
            var request = await _dbContext.RequestsRegisters
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.AuthorityId == _userContext.AuthorityId && r.Id == requestRegisterId)
                ?? throw new BusinessLogicValidationException(Errors.RequestNotFound);

            if (!_userContext.IsExecutive && !_userContext.SmartPAUserId.Equals(request.AuthorUserId))
                throw new BusinessLogicValidationException(Errors.RequestRollbackPermission);

            if (request.ExecutiveDigitalSignStatus.Equals(ExecutiveDigitalSignStatus.NotRequired))
                throw new BusinessLogicValidationException(Errors.RequestRollbackSignNotRequired);

            if (request.ExecutiveDigitalSignStatus.Equals(ExecutiveDigitalSignStatus.Signed))
                throw new BusinessLogicValidationException(Errors.RequestRollbackSignSigned);

            request.ExecutiveDigitalSignStatus = ExecutiveDigitalSignStatus.NotRequired;
            _dbContext.RequestsRegisters.Update(request);
            await _dbContext.SaveChangesAsync();
        }

        // Restituisce le richieste
        async Task<FilterResult<T>> ITemplateService.SearchRegisters<T>(RequestsRegisterFilterCriteria criteria)
        {
            var fc = criteria ?? new();
            var query = _dbContext.RequestsRegisters
                .Include(r => r.Template)
                .Include(r => r.Licensee)
                .AsNoTracking()
                .Where(r => r.AuthorityId == _userContext.AuthorityId);

            query = fc.Id.HasValue ? query.Where(q => q.Id == fc.Id) : query;
            query = fc.LicenseeId.HasValue ? query.Where(q => q.LicenseeId == fc.LicenseeId) : query;
            query = fc.TemplateId.HasValue ? query.Where(q => q.TemplateId == fc.TemplateId) : query;
            query = !string.IsNullOrWhiteSpace(fc.Description) ? query.Where(q => q.Template!.Description!.Contains(fc.Description)) : query;
            query = fc.LastUpdateFrom.HasValue ? query.Where(q => q.LastUpdate >= fc.LastUpdateFrom) : query;
            query = fc.LastUpdateTo.HasValue ? query.Where(q => q.LastUpdate <= fc.LastUpdateTo) : query;
            query = fc.ExecutiveDigitalSignStatus.HasValue ? query.Where(q => q.ExecutiveDigitalSignStatus == fc.ExecutiveDigitalSignStatus) : query;
            query = fc.LicenseeType.HasValue ? query.Where(q => q.Licensee!.Type == fc.LicenseeType) : query;
            query = !string.IsNullOrWhiteSpace(fc.LicenseeNumber) ? query.Where(q => q.Licensee!.Number == fc.LicenseeNumber) : query;

            var result = await query.OrderAndPageAsync(fc.ToTypedCriteria(fc.MapSortCriteria()));

            var accessToken = await GetClientApplicationToken();

            var documentIds = result.Items.Select(d => d.DMSDocumentId!);

            IEnumerable<Document> dmsDocuments;
            try
            {
                var rolePath = (await _organizationalChartService.SearchLevels(_userContext.AuthorityId, new LevelSearchCriteria
                {
                    UserId = _userContext.SmartPAUserId
                }, accessToken)).FirstOrDefault()?.RolePath;

                dmsDocuments = (await _dmsService.SearchDocuments(_userContext.AuthorityId, _userContext.SmartPAUserId.Value, rolePath, new DocumentSearchCriteria
                {
                    DocumentIds = documentIds,
                    OnlyLatestVersions = true
                }, accessToken)).Items;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while downloading documents from dms {ex.Message}");
                throw new BusinessLogicValidationException($"{Errors.RequestsNotRetrieved}<br>Dettaglio: [{ex.Message}]");
            }

            Dictionary<string, RequestDMSInfo> dmsRequestsInfo = new();
            foreach (var doc in dmsDocuments)
            {
                var protocolInfo = doc.Registrations.FirstOrDefault(d => d.RegistrationType == "Protocollo");
                dmsRequestsInfo.Add(doc.DocumentId, new RequestDMSInfo
                {
                    IsSigned = doc.IsSigned,
                    ProtocolNumber = protocolInfo != default ? protocolInfo.Number : null,
                    ProtocolDate = protocolInfo != default ? (DateTime.TryParse(protocolInfo.Date, out DateTime date) ? date : null) : null,
                    FileName = doc.DocumentAttachment?.FileName
                });
            }

            return result.MapFilterResult(m => m.MapGetRequestRegister<T>(dmsRequestsInfo)!);
        }

        // Invia al dirigente le richieste da firmare
        async Task ITemplateService.SendRequests(long licenseeId, RequestRegisterSend requestsToSend)
        {
            requestsToSend?.Validate();

            if (licenseeId <= 0)
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            if (!await _dbContext.Licensees.AsNoTracking().AnyAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == licenseeId))
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            await _sysEventManager.SendAsync(ApplicationsTopics.TaxiDriverEvents, new SystemEventBase
            {
                Payload = new RequestToSignEvent
                {
                    AuthorityId = _userContext.AuthorityId,
                    TenantId = _userContext.TenantId,
                    UserId = _userContext.SmartPAUserId!.Value,
                    ExecutiveEmail = requestsToSend!.ExecutiveEmail,
                    RequestIds = requestsToSend.Ids.ToList(),
                }.JsonSerialize(false),
                Type = TaxiDriverEventTypes.RequestToSign
            });
        }

        async Task ITemplateService.UpdateRequest(long licenseeId, long requestRegisterId)
        {
            if (licenseeId <= 0)
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            if (requestRegisterId <= 0)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            if (!await _dbContext.Licensees.AsNoTracking().AnyAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == licenseeId))
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            var request = await _dbContext.RequestsRegisters.FirstOrDefaultAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == requestRegisterId);

            if (request == default)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            request.ExecutiveDigitalSignStatus = ExecutiveDigitalSignStatus.Required;

            _dbContext.RequestsRegisters.Update(request);
            await _dbContext.SaveChangesAsync();
        }

        // Scarica una richiesta dal documentale
        async Task<DMSFileResult> ITemplateService.DownloadRequestDocument(long requestRegisterId)
        {
            if (requestRegisterId <= 0)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);
            var request = await _dbContext.RequestsRegisters.AsNoTracking().FirstOrDefaultAsync(r => r.AuthorityId == _userContext.AuthorityId && r.Id == requestRegisterId);

            if (request == default)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var documentId = request.DMSDocumentId!;
            var accessToken = await GetClientApplicationToken();
            byte[]? file;
            Document doc;
            try
            {
                var rolePath = (await _organizationalChartService.SearchLevels(_userContext.AuthorityId, new LevelSearchCriteria
                {
                    UserId = _userContext.SmartPAUserId
                }, accessToken)).FirstOrDefault()?.RolePath;

                file = await _dmsService.GetDocumentContent(_userContext.AuthorityId, _userContext.SmartPAUserId.Value, rolePath, documentId, accessToken);
                doc = await _dmsService.GetDocumentById(_userContext.AuthorityId, _userContext.SmartPAUserId.Value, rolePath, documentId, accessToken);

                if (file == null)
                    throw new BusinessLogicValidationException(Errors.RequestNotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while downloading document from dms {ex.Message}");
                throw new BusinessLogicValidationException($"{Errors.RequestNotDownloaded}<br>Dettaglio: [{ex.Message}]");
            }

            return new DMSFileResult
            {
                Content = file,
                FileName = doc.DocumentAttachment?.FileName ?? string.Empty,
                MimeType = MimeTypes.GetMimeType(doc.DocumentAttachment?.FileName)
            };
        }

        // Crea e scarica una bozza della richiesta
        async Task<string> ITemplateService.CreateDraftDocument(long licenseeId, long templateId, AdditionalInformation information)
        {
            information.Validate();

            if (licenseeId <= 0)
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            if (templateId <= 0)
                throw new BusinessLogicValidationException(Errors.Template_TemplateNotFound);

            var template = await _dbContext.Templates.AsNoTracking().FirstOrDefaultAsync(a => a.AuthorityId == _userContext.AuthorityId && a.Id == templateId && !a.Deleted);
            if (template == default || string.IsNullOrWhiteSpace(template.FileId))
                throw new BusinessLogicValidationException(Errors.Template_TemplateNotFound);

            if (!await _dbContext.Licensees.AsNoTracking().AnyAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == licenseeId))
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            using var stream = new MemoryStream(await CreateDoc(licenseeId, template, information));

            var path = $"{_userContext.AuthorityId}/tmp/{Guid.NewGuid()}";
            var fileName = template.FileName!.ToUTF8();
            var mimeType = template.MimeType ?? MimeTypes.GetMimeType(fileName) ?? MimeTypes.MicrosoftOfficeOOXMLWordDocument.Code;

            await _fileStorage.UploadFileAsync(ApplicationsBlobStorages.Taxi, path, stream, mimeType, fileName);

            return (await _fileStorage.DownloadFileAsync(ApplicationsBlobStorages.Taxi, path)).Data;
        }

        // Carica una richiesta direttamente da file nel documentale
        async Task<long> ITemplateService.UploadRequest(long licenseeId, long templateId, AdditionalInformationWrite request)
        {
            request?.Validate();

            //var fileName = request!.Attachment!.FileName![..^(request.Attachment.FileName.LastIndexOf('.'))];
            var fileName = Path.GetFileNameWithoutExtension(request.Attachment.FileName);
            var fileNameExt = request.Attachment.FileName[(request.Attachment.FileName.LastIndexOf('.') + 1)..];

            request.Attachment.FileName = $"{fileName}_{DateTime.UtcNow:yyyyMMddHHmmss}.{fileNameExt}";

            var mimeType = request.Attachment!.MimeType ?? MimeTypes.GetMimeType(request.Attachment.FileName!);

            if (string.IsNullOrWhiteSpace(mimeType))
                throw new BusinessLogicValidationException(string.Format(Errors.InvalidAttachmentExtension, mimeType));

            if ((request.Attachment.Content?.Length ?? 0) <= 0)
                throw new BusinessLogicValidationException(Errors.AttachmentContentIsMandatory);

            if (licenseeId <= 0)
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            if (templateId <= 0)
                throw new BusinessLogicValidationException(Errors.Template_TemplateNotFound);

            var template = await _dbContext.Templates.AsNoTracking().FirstOrDefaultAsync(a => a.AuthorityId == _userContext.AuthorityId && a.Id == templateId && !a.Deleted);
            if (template == default || string.IsNullOrWhiteSpace(template.FileId))
                throw new BusinessLogicValidationException(Errors.Template_TemplateNotFound);

            var licensee = await _dbContext.Licensees.FirstOrDefaultAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == licenseeId)
                ?? throw new BusinessLogicValidationException(Errors.LicenseeNotFound);
            var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.SmartPAUserId == _userContext.SmartPAUserId)
                ?? throw new BusinessLogicValidationException(Errors.UserNotFound);
            var accessToken = await GetClientApplicationToken();
            Document? document;

            try
            {
                var rolePath = (await _organizationalChartService.SearchLevels(_userContext.AuthorityId, new LevelSearchCriteria
                {
                    UserId = _userContext.SmartPAUserId
                }, accessToken)).FirstOrDefault()?.RolePath;

                document = await _dmsService.AddDocument(_userContext.AuthorityId, _userContext.SmartPAUserId!.Value, rolePath,
                    user.MapDocument(licensee, _userContext.SmartPAUserId!.Value, template.Description!, await _fileStorage.GetDocumentTypeId(_userContext.AuthorityId)), accessToken);

                if (string.IsNullOrWhiteSpace(document?.DocumentId))
                    throw new BusinessLogicValidationException(Errors.Template_TemplateNotFound);

                if (!licensee.FolderId.HasValue)
                {
                    await licensee.CreateFolder(_fileStorage, _dmsService, _userContext.AuthorityId, _userContext.SmartPAUserId.Value, rolePath, licensee.Number, licensee.Type, accessToken);
                    _dbContext.Licensees.Update(licensee);
                }

                await _dmsService.AddDocumentToFolder(_userContext.AuthorityId, _userContext.SmartPAUserId.Value, rolePath, document?.DocumentId, licensee.FolderId.Value.ToString(), accessToken);
                await _dmsService.UploadDocumentContent(_userContext.AuthorityId, _userContext.SmartPAUserId!.Value, rolePath, document?.DocumentId, request.Attachment!, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while uploading document in dms {ex.Message}");
                throw new BusinessLogicValidationException($"{Errors.RequestNotUploaded}<br>Dettaglio: [{ex.Message}]");
            }
            var entity = new RequestsRegisterEntity
            {
                AuthorityId = _userContext.AuthorityId,
                LastUpdate = DateTime.UtcNow,
                LicenseeId = licenseeId,
                TemplateId = templateId,
                DMSDocumentId = document?.DocumentId,
                ExecutiveDigitalSignStatus = ExecutiveDigitalSignStatus.NotRequired,
                AuthorUserId = _userContext.SmartPAUserId!.Value
            };
            await _dbContext.RequestsRegisters.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        // Restituisce l'anteprima della richiesta
        async Task<string?> ITemplateService.GetImagePreview(long licenseeId, long templateId)
        {
            var template = await _dbContext.Templates
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == templateId && !a.Deleted);

            if (template == default || string.IsNullOrWhiteSpace(template.FileId))
                throw new BusinessLogicValidationException(Errors.Template_TemplateNotFound);
            var templatePath = $"{_userContext.AuthorityId}/templates/{template.FileId}";

            try
            {
                var metadatavalue = await PrepareDocTokens(licenseeId, new AdditionalInformation());
                using WordDocument word = new(await _fileStorage.DownloadFileAsStreamAsync(ApplicationsBlobStorages.Taxi, templatePath, DateTime.UtcNow), FormatType.Automatic);
                var findField = Regex.Matches(word.GetText(), @"\{(.*?)\}").GroupBy(x => x.Value).Select(x => x.Key).ToDictionary(k => k, v => v.Replace("{", "").Replace("}", ""));
                foreach (var find in findField)
                {
                    object value = null;
                    JToken token = null;
                    try
                    {
                        token = metadatavalue.SelectToken(find.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error while selecting the token using the JsonPath '{find.Value}': {ex.Message}");
                    }

                    if (token != null)
                    {
                        var type = token.Type.ConvertionType();
                        value = token.ToObject(type);

                        if (value is bool boolean)
                        {
                            value = boolean ? "[X]" : "[]";
                        }
                        else if (value is DateTime time)
                        {
                            value = time.ToString("dd/MM/yyyy");
                        }
                        else if (value is string @string
                            && DateTime.TryParseExact(@string, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var data))
                        {
                            value = data.ToString("dd/MM/yyyy");
                        }
                    }
                    word.Replace(find.Key, value?.ToString() ?? string.Empty, true, false);
                }
                using MemoryStream htmlStream = new();
                word.Save(htmlStream, FormatType.Html);
                htmlStream.Position = 0;
                word.Dispose();

                var path = $"{_userContext.AuthorityId}/tmp/{Guid.NewGuid()}_{template.FileName}.htm";
                await _fileStorage.UploadFileAsync(ApplicationsBlobStorages.Taxi, path, htmlStream, MimeTypes.HyperTextMarkupLanguage.Code, $"{template.FileName}.htm");
                return (await _fileStorage.DownloadFileAsync(ApplicationsBlobStorages.Taxi, path)).Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while genereting image preview for template '{templateId}': {ex.Message}");
                return string.Empty;
            }
        }

        async Task ITemplateService.DeleteRequest(long licenseeId, long requestRegisterId)
        {
            if (requestRegisterId <= 0)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var requestEntity = await _dbContext.RequestsRegisters.FirstOrDefaultAsync(r => r.AuthorityId == _userContext.AuthorityId && r.LicenseeId == licenseeId && r.Id == requestRegisterId) ??
                                            throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var accessToken = await GetClientApplicationToken();

            try
            {
                var rolePath = (await _organizationalChartService.SearchLevels(_userContext.AuthorityId, new LevelSearchCriteria
                {
                    UserId = _userContext.SmartPAUserId
                }, accessToken)).FirstOrDefault()?.RolePath;

                await _dmsService.DeleteDocument(_userContext.AuthorityId, _userContext.SmartPAUserId!.Value, rolePath!, requestEntity.DMSDocumentId!, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while downloading document from dms {ex.Message}");
                throw new BusinessLogicValidationException($"{Errors.RequestNotDownloaded}<br>Dettaglio: [{ex.Message}]");
            }

            _dbContext.RequestsRegisters.Remove(requestEntity);
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Private Methods

        async Task<string> GetClientApplicationToken() =>
            await _httpClientFactory.GetClientApplicationToken(_smartPAClientData, _userContext.AuthorityId.ToString(), _userContext.TenantId);

        async Task<byte[]> CreateDoc(long licenseeId, TemplateEntity template, AdditionalInformation information)
        {
            var templatePath = $"{_userContext.AuthorityId}/templates/{template.FileId}";
            var stream = await _fileStorage.DownloadFileAsStreamAsync(ApplicationsBlobStorages.Taxi, templatePath, DateTime.UtcNow);
            return ReplaceDocxAsync(stream, (await PrepareDocTokens(licenseeId, information)));
        }

        async Task<JObject> PrepareDocTokens(long licenseeId, AdditionalInformation information)
        {
            var licensee = await _dbContext.Licensees
                        .Include(l => l.Vehicle)
                        .Include(l => l.LicenseesIssuingOffice)
                        .Include(l => l.LicenseesTaxiDrivers)
                            .ThenInclude(a => a.TaxiDriver)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == licenseeId) ?? new LicenseeEntity();

            var owner = licensee.LicenseesTaxiDrivers?.FirstOrDefault(a => a.TaxiDriverType == DriverTypes.Master)?.TaxiDriver ?? new TaxiDriverEntity();
            var collaborator = licensee.LicenseesTaxiDrivers?.FirstOrDefault(a => a.TaxiDriverType == DriverTypes.FamilyCollaborator)?.TaxiDriver ?? new TaxiDriverEntity();
            var vehicleHistory = _dbContext.LicenseesHistory.OrderByDescending(a => a.SysEndTime)?
                                            .FirstOrDefault(a => a.VariationType == VariationTypes.VehiclesVariation &&
                                                                a.LicenseeId == licenseeId) ?? new LicenseeHistoryEntity();

            var ownerHistoryId = (await _dbContext.LicenseesHistory.OrderByDescending(a => a.SysEndTime)
                                            .FirstOrDefaultAsync(a => a.VariationType == VariationTypes.TaxiDriversVariation &&
                                                                a.LicenseeId == licenseeId && a.TaxiDriverType == DriverTypes.Master))?.TaxiDriverId ?? 0;
            var ownerHystory = (await _dbContext.Drivers.FirstOrDefaultAsync(a => a.Id == ownerHistoryId)) ?? new TaxiDriverEntity();

            var json = (new
            {
                DateNow = DateTime.UtcNow,
                information.InternalProtocol,
                AdditionalInformation = new
                {
                    Year = information.Year.DefaultValue(),
                    Date = information.Date.DateTimeToString(),
                    Day = information.Day.DefaultValue(),
                    Months = information.Months.DefaultValue(),
                    DateFrom = information.DateFrom.DateTimeToString(),
                    DateTo = information.DateTo.DateTimeToString(),
                    ProtocolNumber = information.ProtocolNumber.DefaultValue(),
                    ProtocolDate = information.ProtocolDate.DateTimeToString(),
                    Note = information.Note.DefaultValue(),
                    CollaboratorRelationship = information.CollaboratorRelationship.DefaultValue(),
                    FreeText = information.FreeText.DefaultValue(),
                    InternalProtocolDate = information.InternalProtocolDate.DateTimeToString(),
                    InternalProtocolNumber = information.InternalProtocolNumber.DefaultValue()
                },
                Licensee = new
                {
                    Number = licensee.Number.DefaultValue(),
                    licensee.ReleaseDate,
                    LicenseeIssuingOffice = licensee.LicenseesIssuingOffice?.Description.DefaultValue(),
                    Type = licensee.Type.ToString().DefaultValue(),
                    VehiclePlate = licensee.Vehicle?.LicensePlate.DefaultValue(),
                    VehicleModel = licensee.Vehicle?.Model.DefaultValue(),
                    SubstitutedVehiclePlate = vehicleHistory?.VehicleLicensePlate.DefaultValue(),
                    SubstitutedVehicleModel = vehicleHistory?.VehicleModel.DefaultValue(),
                },
                Owner = new
                {
                    DisplayName = owner.DisplayName.DefaultValue(),
                    FiscalCode = owner.FiscalCode.DefaultValue(),
                    BirthCity = owner.BirthCity.DefaultValue(),
                    BirthProvince = owner.BirthProvince.DefaultValue(),
                    BirthDate = owner.BirthDate.DateTimeToString(),
                    ResidentCity = owner.ResidentCity.DefaultValue(),
                    ResidentAddress = owner.Address.DefaultValue(),
                    ResidentProvince = owner.ResidentProvince.DefaultValue(),
                    o_a = IsMan(owner.FiscalCode) ? "o" : "a",
                    sig_sigra = IsMan(owner.FiscalCode) ? "Sig." : "Sig.ra",
                },
                Collaborator = new
                {
                    DisplayName = collaborator.DisplayName.DefaultValue(),
                    FiscalCode = collaborator.FiscalCode.DefaultValue(),
                    BirthCity = collaborator.BirthCity.DefaultValue(),
                    BirthProvince = collaborator.BirthProvince.DefaultValue(),
                    BirthDate = collaborator.BirthDate.DateTimeToString(),
                    ResidentCity = collaborator.ResidentCity.DefaultValue(),
                    ResidentAddress = collaborator.Address.DefaultValue(),
                    ResidentProvince = collaborator.ResidentProvince.DefaultValue(),
                    O_A = IsMan(collaborator.FiscalCode) ? "o" : "a",
                    Sig_Sigra = IsMan(collaborator.FiscalCode) ? "Sig." : "Sig.ra",
                },
                Substituted = new
                {
                    DisplayName = ownerHystory.DisplayName.DefaultValue(),
                    FiscalCode = ownerHystory.FiscalCode.DefaultValue(),
                    BirthCity = ownerHystory.BirthCity.DefaultValue(),
                    BirthProvince = ownerHystory.BirthProvince.DefaultValue(),
                    BirthDate = ownerHystory.BirthDate.DateTimeToString(),
                    ResidentCity = ownerHystory.ResidentCity.DefaultValue(),
                    ResidentAddress = ownerHystory.Address.DefaultValue(),
                    ResidentProvince = ownerHystory.ResidentProvince.DefaultValue(),
                    O_A = IsMan(ownerHystory.FiscalCode) ? "o" : "a",
                    Sig_Sigra = IsMan(ownerHystory.FiscalCode) ? "Sig." : "Sig.ra",
                }
            });

            return JObject.FromObject(json);
        }

        byte[] ReplaceDocxAsync(Stream stream, JObject metadatavalue)
        {
            using var word = new WordDocument(stream, FormatType.Docx);
            var docText = word.GetText();
            var pattern = @"\{(.*?)\}";
            var matches = Regex.Matches(docText, pattern);
            var findField = matches.GroupBy(x => x.Value).Select(x => x.Key).ToDictionary(k => k, v => v.Replace("{", "").Replace("}", ""));

            foreach (var find in findField)
            {
                var value = default(object?);
                var token = default(JToken?);

                try
                {
                    token = metadatavalue.SelectToken(find.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while selecting the token using the JsonPath '{find.Value}': {ex.Message}");
                }

                if (token != null)
                {
                    var type = token.Type.ConvertionType();

                    value = token.ToObject(type);

                    if (value is bool boolean)
                        value = boolean ? "[X]" : "[]";
                    else if (value is DateTime time)
                        value = time.ToString("dd/MM/yyyy");
                    else if (value is string @string && DateTime.TryParseExact(@string, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var data))
                        value = data.ToString("dd/MM/yyyy");
                }

                word.Replace(find.Key, value?.ToString() ?? string.Empty, true, false);
            }

            using var outputStream = new MemoryStream();

            word.Save(outputStream, FormatType.Docx);
            outputStream.Position = 0;

            var result = outputStream.ToArray();

            word.Close();

            return result;
        }

        static bool IsMan(string? fiscalCode)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(fiscalCode) && fiscalCode.Length > 15 && Convert.ToInt32(fiscalCode.Substring(9, 2)) - 40 < 0;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}