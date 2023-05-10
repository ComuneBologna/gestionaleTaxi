using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.BusinessLogic.Mapper;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.DAL;
using Asf.Taxi.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartTech.Common.ApplicationUsers;
using SmartTech.Common.ApplicationUsers.Models;
using SmartTech.Common.Enums;
using SmartTech.Common.Extensions;
using SmartTech.Common.Models;
using SmartTech.Common.Models.Proxy;
using SmartTech.Common.Services;
using SmartTech.Infrastructure;
using SmartTech.Infrastructure.DataAccessLayer.EFCore;
using SmartTech.Infrastructure.Exceptions;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Search;
using SmartTech.Infrastructure.Validations;
using System.Security.Cryptography;
using System.Text;

namespace Asf.Taxi.BusinessLogic.Services
{
    class DigitalSignService : IDigitalSignService
    {
        readonly ITaxiUserContext _userContext;
        readonly ILogger<ProtocolService> _logger;
        readonly IProxyService _digitalSignService;
        readonly IHttpClientFactory _httpClientFactory;
        readonly SmartPAClientData _smartPAClientData;
        readonly TaxiDriverDBContext _dbContext;
        readonly IOrganizationalChart _organizationalChartService;
        readonly IConfiguration _configuration;
        readonly IUsersService _usersService;

        public DigitalSignService(ITaxiUserContext userContext, ILogger<ProtocolService> logger, IHttpClientFactory httpClientFactory,
            IOptions<SmartPAClientData> smartPAClientData, TaxiDriverDBContext dbContext, IProxyService digitalSignService, IOrganizationalChart organizationalChartService,
            IConfiguration configuration, IUsersService usersService)
        {
            _userContext = userContext;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _smartPAClientData = smartPAClientData.Value;
            _dbContext = dbContext;
            _digitalSignService = digitalSignService;
            _organizationalChartService = organizationalChartService;
            _configuration = configuration;
            _usersService = usersService;
        }

        async Task<DigitalSignCredential?> IDigitalSignService.GetCredential()
        {
            var credential = await _dbContext.Credentials.FirstOrDefaultAsync(c => c.AuthorityId == _userContext.AuthorityId && c.UserId == _userContext.SmartPAUserId!.Value);

            return credential != default ? new()
            {
                Username = credential.Username,
                Password = DecryptString(_configuration.GetSection("SignCredential:PasswordKey").Value, credential.Password!)
            } : default;
        }

        async Task<FilterResult<CredentialUser>> IDigitalSignService.SearchCredentials(CredentialUserFilterCriteria search)
        {
            var sc = search ?? new();
            var query = _dbContext.Credentials.Where(c => c.AuthorityId == _userContext.AuthorityId);

            query = !string.IsNullOrWhiteSpace(sc.FirstName) ? query.Where(c => c.FirstName!.Contains(sc.FirstName)) : query;
            query = !string.IsNullOrWhiteSpace(sc.LastName) ? query.Where(c => c.LastName!.Contains(sc.LastName)) : query;

            var result = await query.OrderAndPageAsync(sc.ToTypedCriteria(sc.MapSortCriteria()));

            return result.MapFilterResult(c => c.Map());
        }

        async Task IDigitalSignService.UpsertCredential(DigitalSignCredential model)
        {
            model?.Validate();

            var credential = await _dbContext.Credentials.FirstOrDefaultAsync(c => c.AuthorityId == _userContext.AuthorityId && c.UserId == _userContext.SmartPAUserId!.Value);
            var encryptedPassword = EncryptString(_configuration.GetSection("SignCredential:PasswordKey").Value, model!.Password!);

            if (credential == default)
            {
                var user = await _usersService.GetSmartPAUserInfo<ApplicationUser>(_userContext.SmartPAUserId!.Value);

                if (user == default)
                    throw new BusinessLogicValidationException(Errors.UserNotFound);

                credential = model.Map(user.FirstName!, user.LastName!, _userContext.AuthorityId, _userContext.SmartPAUserId!.Value, encryptedPassword);
                await _dbContext.Credentials.AddAsync(credential);
            }
            else
            {
                credential.Map(model!.Username!, encryptedPassword);
                _dbContext.Credentials.Update(credential);
            }

            await _dbContext.SaveChangesAsync();
        }

        async Task IDigitalSignService.DeleteCredential()
        {
            var entity = await _dbContext.Credentials.FirstOrDefaultAsync(c => c.AuthorityId == _userContext.AuthorityId && c.UserId == _userContext.SmartPAUserId!.Value)
                ?? throw new BusinessLogicValidationException(Errors.CredentialNotFound);

            _dbContext.Credentials.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        async Task IDigitalSignService.SendOTP(DigitalSignCredential userCredential)
        {
            var accessToken = await GetClientApplicationToken();

            try
            {
                await _digitalSignService.SendOTP(_userContext.AuthorityId, _userContext.ApplicationId ?? 12, userCredential, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while sending OTP {ex.Message}");
                throw new BusinessLogicValidationException($"{Errors.RequestOTPNotSended}<br>Dettaglio: [{ex.Message}]");
            }
        }

        async Task<IEnumerable<ErrorModel>> IDigitalSignService.SignMultipleDocuments(SignTypes signType, MultipleDigitalSignWrite multipleDigitalSign)
        {
            multipleDigitalSign?.Validate();

            var requests = await _dbContext.RequestsRegisters
                .Include(r => r.Licensee)
                    .ThenInclude(l => l!.LicenseesTaxiDrivers)
                        .ThenInclude(lt => lt.TaxiDriver)
                .Where(r => r.AuthorityId == _userContext.AuthorityId && multipleDigitalSign!.RequestRegisterIds.Contains(r.Id))
                .ToListAsync();

            var accessToken = await GetClientApplicationToken();
            var rolePath = (await _organizationalChartService.SearchLevels(_userContext.AuthorityId, new LevelSearchCriteria
            {
                UserId = _userContext.SmartPAUserId
            }, accessToken)).FirstOrDefault()?.RolePath;

            List<MultipleDigitalSignData> signData = new();
            foreach (var rs in requests.ToLookup(k => k.LicenseeId).ToList())
            {
                var applicationDocumentCode = $"TaxiDocument_{_userContext.ApplicationId}_{rs.Key}_{rs.First().Licensee.Number}_code";
                signData.AddRange(rs.ToList().Select(r => new MultipleDigitalSignData
                {
                    DocumentId = r.DMSDocumentId,
                    ApplicationDocumentCode = applicationDocumentCode,
                    AdditionalData = new List<KeyValueModel>
                    {
                        new KeyValueModel
                        {
                            Key = "LicenseeType",
                            Value = r.Licensee!.Type.Equals(LicenseeTypes.Taxi) ? "TAXI" : r.Licensee.Type.Equals(LicenseeTypes.NCC_Auto) ? "NCC" : "ALTRO"
                        },
                    new KeyValueModel
                    {
                        Key = "LicenseeNumber",
                            Value = r.Licensee.Number
                        },
                        new KeyValueModel
                        {
                            Key = "LicenseeOwner",
                            Value = r.Licensee.LicenseesTaxiDrivers.Select(ltd => $"{ltd.TaxiDriver.FirstName} {ltd.TaxiDriver.LastName}").FirstOrDefault()
                        }
                    }
                }).ToList());
            }
            List<ErrorModel> signResults = new();

            try
            {
                signResults = (await _digitalSignService.Sign(_userContext.AuthorityId, _userContext.ApplicationId!.Value, rolePath, _userContext.SmartPAUserId!.Value,
                    new MultipleDigitalSign
                    {
                        Credential = multipleDigitalSign!.Credential,
                        SignData = signData
                    }, signType, accessToken)).ToList();
            }
            catch (Exception ex)
            {
                signResults.AddRange(signData.Select(d => new ErrorModel
                {
                    Error = ex.Message,
                    ItemId = d.DocumentId,
                    Success = false
                }));
            }

            if (signResults.Any())
            {
                foreach (var req in requests)
                {
                    var result = signResults.FirstOrDefault(e => e.ItemId == req.DMSDocumentId);
                    if (result != default)
                    {
                        req.DigitalSignResult = result.Error;
                        req.LastUpdate = DateTime.UtcNow;
                        if (_userContext.IsExecutive && result.Success)
                            req.ExecutiveDigitalSignStatus = ExecutiveDigitalSignStatus.Signed;
                    }
                }

                _dbContext.RequestsRegisters.UpdateRange(requests);
                await _dbContext.SaveChangesAsync();
            }

            return signResults;
        }

        async Task<string> IDigitalSignService.GetDigitalSignError(long requestRegisterId)
        {
            var request = await _dbContext.RequestsRegisters.AsNoTracking().FirstOrDefaultAsync(r => r.AuthorityId == _userContext.AuthorityId && r.Id == requestRegisterId) ??
                                    throw new BusinessLogicValidationException(Errors.RequestNotFound);

            return !string.IsNullOrWhiteSpace(request.DigitalSignResult) ? request.DigitalSignResult : Errors.DocumentToSign;
        }

        #region Private Methods

        static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using MemoryStream memoryStream = new();
                using CryptoStream cryptoStream = new((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
                using (StreamWriter streamWriter = new((Stream)cryptoStream))
                {
                    streamWriter.Write(plainText);
                }

                array = memoryStream.ToArray();
            }

            return Convert.ToBase64String(array);
        }

        static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new(buffer);
            using CryptoStream cryptoStream = new((Stream)memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new((Stream)cryptoStream);
            return streamReader.ReadToEnd();
        }

        async Task<string> GetClientApplicationToken() =>
            await _httpClientFactory.GetClientApplicationToken(_smartPAClientData, _userContext.AuthorityId.ToString(), _userContext.TenantId);

        #endregion
    }
}