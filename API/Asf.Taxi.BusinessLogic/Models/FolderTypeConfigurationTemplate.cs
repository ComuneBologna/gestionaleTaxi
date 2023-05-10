using SmartTech.Common.Enums;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class DMSConfigurationtemplate
	{
		public FolderTypeConfigurationTemplate? FolderTypeConfiguration { get; set; }

		public DocumentTypeConfigurationTemplate? DocumentTypeConfiguration { get; set; }
	}

	public class FolderTypeConfigurationTemplate
	{
		public string? Id { get; set; }

		[Required]
		public long? ClassificationSchemeId { get; set; }

		[Required]
		public string? Name { get; set; }

		[Required]
		public ArchivalTypeTypes? ArchiveType { get; set; }

		public IEnumerable<string> MetadataKeys { get; set; } = Enumerable.Empty<string>();
	}

	public class DocumentTypeConfigurationTemplate
	{
		[Required]
		public string? DocumentTypeId { get; set; }
	}
}