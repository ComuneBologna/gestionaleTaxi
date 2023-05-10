using SmartTech.Common;
using SmartTech.Common.Models;
using SmartTech.Common.Services;
using SmartTech.Infrastructure;
using SmartTech.Infrastructure.Storage;

namespace Asf.Taxi.BusinessLogic.Services
{
	class ExportService : IExportService
	{
		readonly IImportExportService _importExportService;
		readonly IFileStorage _fileStorage;

		public ExportService(IImportExportService importExportService, IFileStorage fileStorage)
		{
			_importExportService = importExportService;
			_fileStorage = fileStorage;
		}

		async Task<string> IExportService.GetExcelExport(IEnumerable<object> objectToExport, string worksheetName)
		{
			var configuration = new List<ExportConfiguration>()
			{
				new ExportConfiguration()
				{
					AutoAdjustColumns = true,
					ExcelHeaderText = Array.Empty<string>(),
					FirstColumnStartIndex = 1,
					FirstRowStartIndex = 1,
					GetAllBorders = true,
					GetDataTable = true,
					GetHeader = true,
					GetHeaderFromClass = true,
					HeaderBold = true,
					HeaderFontSize = 16,
					WoorkSheetName = worksheetName,
					RenderList = objectToExport.ToList()
				}
			};

			var byteArray = _importExportService.GetExportBytes(configuration);
			var fileName = $"{worksheetName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
			var filePath = $"export/{fileName}";

			using Stream stream = new MemoryStream(byteArray);
			
			await _fileStorage.UploadFileAsync(ApplicationsBlobStorages.Taxi, filePath, stream, MimeTypes.MicrosoftOfficeOOXMLSpreadsheet.Code, fileName);
			
			return (await _fileStorage.DownloadFileAsync(ApplicationsBlobStorages.Taxi, filePath)).Data;
		}
	}
}