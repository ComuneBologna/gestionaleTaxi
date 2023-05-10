using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.DAL.Entities;
using Asf.Taxi.DAL.Enums;
using SmartTech.Common;
using SmartTech.Common.Models;
using SmartTech.Common.Services;
using SmartTech.Infrastructure;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Storage;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Extensions
{
	public static class DMSTemplateExtensions
	{
		static readonly string _folderConfigurationTemplate = "DMSConfigurationTemplate.json";

		public static async Task CreateFolder(this LicenseeEntity licenseeEntity, IFileStorage fileStorage, IDocumentManagementSystemService dms,
			long authorityId, Guid smartPAUserId, string rolePath, string licenseeNumber, LicenseeTypes licenseeType, string accessToken)
		{
			if (licenseeEntity.FolderId.HasValue)
				return;

			var dmsTemplate = await fileStorage.DownloadJsonAsync<DMSConfigurationtemplate>(ApplicationsBlobStorages.Taxi, GetFolderConfigurationPath(authorityId), default) ?? new();
			var folderTemplate = dmsTemplate.FolderTypeConfiguration;

			folderTemplate?.Validate();

			if (string.IsNullOrWhiteSpace(folderTemplate!.Id))
			{
				var folderType = await dms.AddFolderType(authorityId, smartPAUserId, new FolderTypeWrite
				{
					ArchivalType = folderTemplate.ArchiveType.Value,
					ClassificationSchemeLevelId = folderTemplate.ClassificationSchemeId,
					Metadata = folderTemplate.MetadataKeys,
					Name = folderTemplate.Name
				}, accessToken);

				folderTemplate.Id = folderType.FolderTypeId;
				dmsTemplate.FolderTypeConfiguration = folderTemplate;
				await fileStorage.UploadJsonAsync(ApplicationsBlobStorages.Taxi, GetFolderConfigurationPath(authorityId), dmsTemplate, _folderConfigurationTemplate);
			}

			var folderSubject = $"{authorityId}_{licenseeNumber}_{licenseeType}";
			var folder = await dms.AddFolder(authorityId, smartPAUserId, rolePath, new FolderWrite
			{
				FolderTypeId = folderTemplate.Id,
				Metadata = folderTemplate.MetadataKeys.Select(mk => new KeyValueModel
				{
					Key = mk,
					Value = mk.Equals("LicenseeNumber") ? licenseeNumber : licenseeType.ToString()
				}),
				Subject = folderSubject
			}, accessToken);

			licenseeEntity.FolderId = Guid.Parse(folder.Id);
		}

		public static async Task<string?> GetDocumentTypeId(this IFileStorage fileStorage, long authorityId)
		{
			var dmsTemplate = await fileStorage.DownloadJsonAsync<DMSConfigurationtemplate>(ApplicationsBlobStorages.Taxi, GetFolderConfigurationPath(authorityId), default) ?? new();
			var documentTemplate = dmsTemplate.DocumentTypeConfiguration;

			documentTemplate?.Validate();

			return documentTemplate.DocumentTypeId;
		}

		static string GetFolderConfigurationPath(long authorityId) => $"{authorityId}/{_folderConfigurationTemplate}";
	}
}