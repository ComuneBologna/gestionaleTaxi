using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.BusinessLogic.Mapper;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Models.ProtocolEmails;
using Asf.Taxi.BusinessLogic.Models.Requests;
using Asf.Taxi.DAL;
using Asf.Taxi.DAL.Entities;
using Asf.Taxi.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartTech.Common.Authentication;
using SmartTech.Common.Enums;
using SmartTech.Common.Extensions;
using SmartTech.Common.Models;
using SmartTech.Common.Services;
using SmartTech.Infrastructure;
using SmartTech.Infrastructure.DataAccessLayer.EFCore;
using SmartTech.Infrastructure.Exceptions;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Search;
using SmartTech.Infrastructure.Validations;
using System.Text.RegularExpressions;

namespace Asf.Taxi.BusinessLogic.Services
{
    class ProtocolService : IProtocolService
    {
        readonly IUserContext _userContext;
        readonly ILogger<ProtocolService> _logger;
        readonly IProxyService _protocolService;
        readonly IHttpClientFactory _httpClientFactory;
        readonly SmartPAClientData _smartPAClientData;
        readonly TaxiDriverDBContext _dbContext;
        readonly IOrganizationalChart _organizationalChartService;

        public ProtocolService(IUserContext userContext, ILogger<ProtocolService> logger, IHttpClientFactory httpClientFactory,
            IOptions<SmartPAClientData> smartPAClientData, TaxiDriverDBContext dbContext, IProxyService protocolService, IOrganizationalChart organizationalChartService)
        {
            _userContext = userContext;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _smartPAClientData = smartPAClientData.Value;
            _dbContext = dbContext;
            _protocolService = protocolService;
            _organizationalChartService = organizationalChartService;
        }

        async Task<ProtocolDataOutput> IProtocolService.ProtocolRequestDocument(long requestRegisterId, ProtocolInput protocol)
        {
            if (requestRegisterId <= 0)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);
            var request = await _dbContext.RequestsRegisters
                                    .Include(rr => rr.Licensee)
                                        .ThenInclude(l => l.LicenseesTaxiDrivers.Where(ltd => ltd.TaxiDriverType == DriverTypes.Master))
                                            .ThenInclude(ltd => ltd.TaxiDriver)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(r => r.AuthorityId == _userContext.AuthorityId && r.Id == requestRegisterId);

            if (request == default)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var documentId = request.DMSDocumentId!;
            var accessToken = await GetClientApplicationToken();

            ProtocolDataOutput protocolData;
            try
            {
                var applicationDocumentCode = $"TaxiDocument_{_userContext.ApplicationId}_{request.LicenseeId}_{request.Licensee.Number}_code";
                var rolePath = (await _organizationalChartService.SearchLevels(_userContext.AuthorityId, new LevelSearchCriteria
                {
                    UserId = _userContext.SmartPAUserId
                }, accessToken)).FirstOrDefault()?.RolePath;

                protocolData = await _protocolService.Protocol(_userContext.AuthorityId, _userContext.ApplicationId.Value, rolePath, applicationDocumentCode, _userContext.SmartPAUserId!.Value,
                    new ProtocolDataInput
                    {
                        AdditionalData = new List<KeyValueModel>
                        {
                            new KeyValueModel
                            {
                                Key = "LicenseeType",
                                Value = request.Licensee.Type.Equals(LicenseeTypes.Taxi) ? "TAXI" : request.Licensee.Type.Equals(LicenseeTypes.NCC_Auto) ? "NCC" : "ALTRO"
                            },
                            new KeyValueModel
                            {
                                Key = "LicenseeNumber",
                                Value = request.Licensee.Number
                            },
                            new KeyValueModel
                            {
                                Key = "LicenseeOwner",
                                Value = request.Licensee.LicenseesTaxiDrivers.Select(ltd => $"{ltd.TaxiDriver.FirstName} {ltd.TaxiDriver.LastName}").FirstOrDefault()
                            }
                        },
                        DocumentId = documentId,
                        Subject = protocol.Subject,
                        Recipient = protocol.Recipient
                    }, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while doing protocol operation {ex.Message}");
                throw new BusinessLogicValidationException($"{Errors.RequestNotProtocol}<br>Dettaglio: [{ex.Message}]");
            }

            if (protocolData == default)
                throw new BusinessLogicValidationException(Errors.RequestNotProtocol);

            request.LastUpdate = protocolData.DateProtocolUTC ?? DateTime.UtcNow;

            _dbContext.RequestsRegisters.Update(request);
            await _dbContext.SaveChangesAsync();

            return protocolData;
        }

        async Task IProtocolService.SendProtocolEmail(long requestRegisterId, ProtocolSendMailInput protocolMail)
        {
            if (requestRegisterId <= 0)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);
            var request = await _dbContext.RequestsRegisters.AsNoTracking().FirstOrDefaultAsync(r => r.AuthorityId == _userContext.AuthorityId && r.Id == requestRegisterId);

            if (request == default)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            protocolMail.Recipients = protocolMail.Recipients.Except(protocolMail.Recipients.Where(r => !r.CheckValidMail())).ToList();

            var documentId = request.DMSDocumentId!;
            var accessToken = await GetClientApplicationToken();

            try
            {
                var rolePath = (await _organizationalChartService.SearchLevels(_userContext.AuthorityId, new LevelSearchCriteria
                {
                    UserId = _userContext.SmartPAUserId
                }, accessToken)).FirstOrDefault()?.RolePath;

                await _protocolService.SendMail(_userContext.AuthorityId, _userContext.ApplicationId.Value, rolePath, _userContext.SmartPAUserId.Value, documentId, protocolMail, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while sending protocol email {ex.Message}");
                throw new BusinessLogicValidationException($"{Errors.RequestProtocolNotSent}<br>Dettaglio: [{ex.Message}]");
            }

            try
            {
                var recipients = protocolMail.Recipients.Except(await _dbContext.Recipients.Where(r => r.AuthorityId == _userContext.AuthorityId && r.LicenseeId == request.LicenseeId)
                                                .Select(r => r.Mail).ToListAsync())
                                                .Select(r => new RecipientEntity()
                                                {
                                                    AuthorityId = _userContext.AuthorityId,
                                                    LicenseeId = request.LicenseeId,
                                                    Mail = r
                                                }).ToList();
                await _dbContext.Recipients.AddRangeAsync(recipients);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while storing recipients email {ex.Message}");
            }
        }

        async Task<EmailStatus> IProtocolService.GetProtocolEmailStatus(long requestRegisterId)
        {
            if (requestRegisterId <= 0)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);
            var request = await _dbContext.RequestsRegisters.AsNoTracking().FirstOrDefaultAsync(r => r.AuthorityId == _userContext.AuthorityId && r.Id == requestRegisterId);

            if (request == default)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var documentId = request.DMSDocumentId!;
            var accessToken = await GetClientApplicationToken();

            try
            {
                return await _protocolService.GetMailStatus(_userContext.AuthorityId, _userContext.ApplicationId!.Value, _userContext.SmartPAUserId!.Value, documentId, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while checking mail status {ex.Message}");
                return default;
            }
        }

        async Task IProtocolService.StartProcess(long requestRegisterId, long processTypeCode)
        {
            if (requestRegisterId <= 0)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);
            var request = await _dbContext.RequestsRegisters.AsNoTracking().FirstOrDefaultAsync(r => r.AuthorityId == _userContext.AuthorityId && r.Id == requestRegisterId);

            if (request == default)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var documentId = request.DMSDocumentId!;
            var accessToken = await GetClientApplicationToken();

            try
            {
                await _protocolService.StartProcess(_userContext.AuthorityId, _userContext.ApplicationId!.Value, documentId, processTypeCode, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while sending protocol email {ex.Message}");
                throw new BusinessLogicValidationException($"{Errors.ProcessNotStarted}<br>Dettaglio: [{ex.Message}]");
            }
        }

        async Task IProtocolService.StopProcess(long requestRegisterId, long processTypeCode)
        {
            if (requestRegisterId <= 0)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var request = await _dbContext.RequestsRegisters.AsNoTracking().FirstOrDefaultAsync(r => r.AuthorityId == _userContext.AuthorityId && r.Id == requestRegisterId);

            if (request == default)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var documentId = request.DMSDocumentId!;
            var accessToken = await GetClientApplicationToken();

            try
            {
                await _protocolService.StopProcess(_userContext.AuthorityId, _userContext.ApplicationId!.Value, documentId, processTypeCode, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while sending protocol email {ex.Message}");
                throw new BusinessLogicValidationException($"{Errors.ProcessNotStopped}<br>Dettaglio: [{ex.Message}]");
            }
        }

        async Task IProtocolService.ReleaseInChargeOfTheFolder(long requestRegisterId)
        {
            if (requestRegisterId <= 0)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var request = await _dbContext.RequestsRegisters.AsNoTracking().FirstOrDefaultAsync(r => r.AuthorityId == _userContext.AuthorityId && r.Id == requestRegisterId);

            if (request == default)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var documentId = request.DMSDocumentId!;
            var accessToken = await GetClientApplicationToken();

            try
            {
                await _protocolService.ReleaseInChargeOfTheFolder(_userContext.AuthorityId, _userContext.ApplicationId!.Value, documentId, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while sending protocol email {ex.Message}");
                throw new BusinessLogicValidationException($"{Errors.InChargeOfTheFolderNotReleased}<br>Dettaglio: [{ex.Message}]");
            }

        }

        async Task<FilterResult<ProxyDocument>> IProtocolService.SearchExternalDocuments(ProxyDocumentSearchCriteria searchCriteria)
        {
            var accessToken = await GetClientApplicationToken();

            return await _protocolService.SearchExternalDocuments(_userContext.AuthorityId, _userContext.ApplicationId.Value, searchCriteria, accessToken);
        }

        async Task<IEnumerable<ProxyExternalDocumentAttachment>> IProtocolService.GetExternalAttachmentsByExternalId(string externalDocumentId)
        {
            var accessToken = await GetClientApplicationToken();

            return await _protocolService.GetExternalAttachmentsByExternalId(_userContext.AuthorityId, _userContext.ApplicationId.Value, externalDocumentId, accessToken);
        }

        async Task IProtocolService.GroupSmartPADocumentToLeadDocument(long requestRegisterId, LeadProtocolDataInput leadDocument)
        {
            if (requestRegisterId <= 0)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var request = await _dbContext.RequestsRegisters.AsNoTracking().FirstOrDefaultAsync(r => r.AuthorityId == _userContext.AuthorityId && r.Id == requestRegisterId);

            if (request == default)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var documentId = request.DMSDocumentId!;
            var accessToken = await GetClientApplicationToken();

            try
            {
                var rolePath = (await _organizationalChartService.SearchLevels(_userContext.AuthorityId, new LevelSearchCriteria
                {
                    UserId = _userContext.SmartPAUserId
                }, accessToken)).FirstOrDefault()?.RolePath;

                await _protocolService.GroupSmartPADocumentToLeadDocument(_userContext.AuthorityId, _userContext.ApplicationId!.Value,
                    rolePath!, _userContext.SmartPAUserId!.Value, documentId, leadDocument, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while sending protocol email {ex.Message}");
                throw new BusinessLogicValidationException($"{Errors.DocumentNotGrouped}<br>Dettaglio: [{ex.Message}]");
            }
        }

        async Task<LeadProtocolDataInput> IProtocolService.GetLeadDocument(long requestRegisterId)
        {
            if (requestRegisterId <= 0)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var request = await _dbContext.RequestsRegisters.AsNoTracking().FirstOrDefaultAsync(r => r.AuthorityId == _userContext.AuthorityId && r.Id == requestRegisterId);

            if (request == default)
                throw new BusinessLogicValidationException(Errors.RequestNotFound);

            var documentId = request.DMSDocumentId!;
            var accessToken = await GetClientApplicationToken();

            try
            {
                return await _protocolService.GetLeadDocument(_userContext.AuthorityId, _userContext.ApplicationId!.Value, documentId, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while sending protocol email {ex.Message}");
                throw new BusinessLogicValidationException($"{Errors.DocumentNotGrouped}<br>Dettaglio: [{ex.Message}]");
            }
        }

        async Task<IEnumerable<ProcessCode>> IProtocolService.GetProcessCodes(string? fullTextSearch)
        {
            var query = _dbContext.ProcessCodes.Where(pc => pc.AuthorityId == _userContext.AuthorityId).AsNoTracking();

            query = !string.IsNullOrWhiteSpace(fullTextSearch) ? query.Where(pc => pc.FullTextSearch!.Contains(fullTextSearch)) : query;

            return (await query.ToListAsync()).Select(pc => new ProcessCode
            {
                Code = pc.Code,
                Description = pc.Description,
                FullTextSearch = pc.FullTextSearch,
                Id = pc.Id
            });
        }

        #region ProtocolEmails
        async Task<long> IProtocolService.AddProtocolEmail(ProtocolEmailWrite protocolEmailWrite)
        {
            protocolEmailWrite?.Validate();

            if (await _dbContext.ProtocolEmails.AnyAsync(ul => ul.AuthorityId == ul.AuthorityId && ul.Email == protocolEmailWrite!.Email))
                throw new BusinessLogicValidationException(Errors.ProtocolEmailAlreadyExists);

            var entity = protocolEmailWrite!.MapPostProtocolEmail(_userContext.AuthorityId);

            await _dbContext.ProtocolEmails.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        async Task IProtocolService.UpdateProtocolEmail(long id, ProtocolEmailUpdate protocolEmailUpdate)
        {
            protocolEmailUpdate?.Validate();

            if (id <= 0)
                throw new BusinessLogicValidationException(Errors.ProtocolEmailNotFound);

            if (await _dbContext.ProtocolEmails.AnyAsync(ul => ul.AuthorityId == ul.AuthorityId && ul.Email == protocolEmailUpdate!.Email && ul.Id != id))
                throw new BusinessLogicValidationException(Errors.ProtocolEmailAlreadyExists);

            var entity = await _dbContext.ProtocolEmails.FirstOrDefaultAsync(ul => ul.AuthorityId == _userContext.AuthorityId && ul.Id == id)
                ?? throw new BusinessLogicValidationException(Errors.ProtocolEmailNotFound);

            entity.MapUpdateProtocolEmail(protocolEmailUpdate!);

            _dbContext.ProtocolEmails.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        async Task IProtocolService.DeleteProtocolEmail(long id)
        {
            if (id <= 0)
                throw new BusinessLogicValidationException(Errors.ProtocolEmailNotFound);

            var entity = await _dbContext.ProtocolEmails.FirstOrDefaultAsync(ul => ul.AuthorityId == _userContext.AuthorityId && ul.Id == id)
                ?? throw new BusinessLogicValidationException(Errors.ProtocolEmailNotFound);

            _dbContext.ProtocolEmails.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        async Task<FilterResult<T>> IProtocolService.SearchProtocolEmails<T>(ProtocolEmailSearchCriteria criteria)
        {
            var sc = criteria ?? new ProtocolEmailSearchCriteria();

            var query = _dbContext.ProtocolEmails
                .Where(w => w.AuthorityId == _userContext.AuthorityId)
                .AsNoTracking();

            query = !sc.Id.HasValue ? query : query.Where(q => q.Id == sc.Id);
            query = (!sc.Ids?.Any() ?? true) ? query : query.Where(q => sc.Ids!.Contains(q.Id));
            query = string.IsNullOrWhiteSpace(sc.Email) ? query : query.Where(q => q.Email!.Contains(sc.Email));
            query = string.IsNullOrWhiteSpace(sc.Description) ? query : query.Where(q => q.Description.Contains(sc.Description));
            query = !sc.Active.HasValue ? query : query.Where(q => q.Active == sc.Active);

            var result = await query.OrderAndPageAsync(sc.ToTypedCriteria(sc.MapProtocolEmailSortCriteria()));
            return result.MapFilterResult(m => m.MapGetProtocolEmail<T>()!);
        }
        #endregion

        #region Private Methods

        async Task<string> GetClientApplicationToken() =>
            await _httpClientFactory.GetClientApplicationToken(_smartPAClientData, _userContext.AuthorityId.ToString(), _userContext.TenantId);

        #endregion
    }
}