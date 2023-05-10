using Asf.Taxi.BusinessLogic;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using SmartTech.Common.API;
using SmartTech.Infrastructure.Attachments;
using SmartTech.Common.Web.Security;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.API.Controllers
{
	public class ImportsController : APIControllerBase
	{
		public ImportsController()
		{
		}

		[HttpGet]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task<FilterResult<TaxiImportInfo>> Get([FromQuery] ImportFilterCriteria search) => await Task.FromResult(new FilterResult<TaxiImportInfo>());

		[HttpPut("{importId}/{status}")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task Put(long importId, ImportStatus status) => await Task.CompletedTask;

		[HttpPost("upload")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task<AttachmentInfo> UploadFile([FromForm] IFormFile file)
		{
			var attachment = (file ?? (await Request.ReadFormAsync()).Files[0]).Map();

			return await Task.FromResult(new AttachmentInfo()
			{
				FileName = attachment.FileName,
				Id = Guid.NewGuid().ToString(),
				MimeType = attachment.MimeType
			});
		}

		[HttpPost]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
		public async Task<long> Post([FromBody] TaxiImportWrite import) => await Task.FromResult(Random.Shared.NextInt64(1, long.MaxValue));

		[HttpGet("{importId}/errors")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator, TaxiPermissions.Taxi_Driver)]
		public async Task<string> GetFile(long importId) => await Task.FromResult(string.Empty);

		[HttpGet("{importId}/excelerrors")]
		[PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator, TaxiPermissions.Taxi_Driver)]
		public async Task<string> GetExcelFile(long importId) => await Task.FromResult(string.Empty);
	}
}