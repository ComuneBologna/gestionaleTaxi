using Asf.Taxi.BusinessLogic.Models.Templates;
using Asf.Taxi.BusinessLogic.Models.Requests;
using Asf.Taxi.DAL.Entities;
using SmartTech.Common.ApplicationUsers.Entities;
using SmartTech.Common.Models;
using SmartTech.Infrastructure.Extensions;
using System.Linq.Expressions;
using SmartTech.Infrastructure;
using SmartTech.Common.Enums;

namespace Asf.Taxi.BusinessLogic.Mapper
{
    public static class TemplateMapper
    {
        public static Expression<Func<TemplateEntity, dynamic>> MapSortCriteria(this TemplatesFilterCriteria tfc) =>
            tfc.KeySelector.FirstCharToUpper() switch
            {
                nameof(TemplateInfo.Description) => x => x.Description!,
                _ => x => x.Id
            };

        public static T MapGetTemplate<T>(this TemplateEntity template) where T : TemplateBase =>
                typeof(T).Equals(typeof(TemplateInfo)) ?
                new TemplateInfo()
                {
                    Description = template.Description,
                    Id = template.Id
                } as T :
                typeof(T).Equals(typeof(TemplateDetails)) ?
                new TemplateDetails
                {
                    Description = template.Description,
                    Id = template.Id,
                    Filename = template.FileName
                } as T :
                new TemplateBase
                {
                    Description = template.Description
                } as T;

        public static TemplateEntity MapPostTemplate(this TemplateWrite template, long authorityId) =>
                new()
                {
                    AuthorityId = authorityId,
                    LastUpdate = DateTime.UtcNow,
                    Description = template.Description,
                    FileId = template.FileId,
                    Deleted = false
                };

        public static Expression<Func<RequestsRegisterEntity, dynamic>> MapSortCriteria(this RequestsRegisterFilterCriteria tfc) =>
            tfc.KeySelector.FirstCharToUpper() switch
            {
                nameof(RequestRegister.TemplateDescription) => x => x.Template!.Description!,
                nameof(RequestRegister.LastUpdate) => x => x.LastUpdate,
                nameof(RequestRegister.ExecutiveDigitalSignStatus) => x => x.ExecutiveDigitalSignStatus,
                _ => x => x.Id
            };

        public static T? MapGetRequestRegister<T>(this RequestsRegisterEntity registry, Dictionary<string, RequestDMSInfo> dmsRequestsInfo) where T : RequestRegisterBase =>
            typeof(T).Equals(typeof(RequestRegister)) ?
            new RequestRegister()
            {
                LastUpdate = registry.LastUpdate,
                TemplateDescription = registry.Template!.Description,
                LicenseeId = registry.LicenseeId,
                TemplateId = registry.TemplateId,
                Id = registry.Id,
                TemplateFileName = dmsRequestsInfo.ContainsKey(registry.DMSDocumentId!) ? dmsRequestsInfo[registry.DMSDocumentId!].FileName : null,
                IsSigned = dmsRequestsInfo.ContainsKey(registry.DMSDocumentId!) ? dmsRequestsInfo[registry.DMSDocumentId!].IsSigned : false,
                ProtocolDate = dmsRequestsInfo.ContainsKey(registry.DMSDocumentId!) ? dmsRequestsInfo[registry.DMSDocumentId!].ProtocolDate : null,
                ProtocolNumber = dmsRequestsInfo.ContainsKey(registry.DMSDocumentId!) ? dmsRequestsInfo[registry.DMSDocumentId!].ProtocolNumber : null,
                ExecutiveDigitalSignStatus = registry.ExecutiveDigitalSignStatus,
                LicenseeNumber = registry.Licensee!.Number,
                LicenseeType = registry.Licensee!.Type
            } as T :
            typeof(T).Equals(typeof(RequestRegisterInfo)) ?
                new RequestRegisterInfo()
                {
                    LastUpdate = registry.LastUpdate,
                    TemplateDescription = registry.Template!.Description,
                    Id = registry.Id,
                    TemplateFileName = dmsRequestsInfo.ContainsKey(registry.DMSDocumentId!) ? dmsRequestsInfo[registry.DMSDocumentId!].FileName : null,
                    LicenseeNumber = registry.Licensee!.Number,
                    LicenseeType = registry.Licensee!.Type
                } as T :
                new RequestRegisterBase()
                {
                    TemplateDescription = registry.Template!.Description,
                    Id = registry.Id,
                    LicenseeNumber = registry.Licensee!.Number,
                    LicenseeType = registry.Licensee!.Type
                } as T;

        public static DocumentWrite MapDocument(this UserEntity user, LicenseeEntity licensee, Guid smartPAUserId, string templateDescription, string documentTypeId)
            => new()
            {
                CreationType = CreationTypes.CREAZIONE_SOFTWARE,
                DocumentTypeId = documentTypeId,
                Subject = templateDescription,
                Metadata = new List<KeyValueModel>
                {
                    new KeyValueModel
                    {
                        Key = "LicenseeNumber", Value = licensee.Number
                    },
                    new KeyValueModel
                    {
                        Key = "LicenseeType", Value = licensee.Type.ToString()
                    }
                }
            };

        public static string DateTimeToString(this DateTime? input) => input.HasValue ? input.Value.ToString("dd/MM/yyyy") : "___/___/_________";

        public static string DefaultValue(this string? input) => string.IsNullOrWhiteSpace(input) ? "________" : input;
    }
}