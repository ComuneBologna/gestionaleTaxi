using Asf.Taxi.BusinessLogic;
using Asf.Taxi.BusinessLogic.Events;
using Asf.Taxi.BusinessLogic.Services;
using Asf.Taxi.DAL.Enums;
using Asf.Taxi.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartTech.Common;
using SmartTech.Infrastructure;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Functions;
using SmartTech.Infrastructure.Storage;
using SmartTech.Infrastructure.SystemEvents;
using System.Globalization;
using System.Text;

namespace Asf.Taxi.Functions
{
	public static class VariationStoreFunction
	{
		[Function("VariationStoreFunction")]
		public static async Task Run([ServiceBusTrigger(ApplicationsTopics.TaxiDriverEventsName, SubscriptionNames.TaxiDriverVariations, Connection = "servicebusconnection")] SystemEventBase sysEvent, FunctionContext executionContext)
		{
			var type = sysEvent.Type ?? string.Empty;
			var log = executionContext.GetLogger("VariationStoreFunction");

			log.LogInformation($"C# ServiceBus topic trigger function processed message: {sysEvent.JsonSerialize()}");
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("it-IT");

			switch (type)
			{
				case TaxiDriverEventTypes.TaxiDriverAudit:
					var auditEvent = sysEvent.Payload.JsonDeserialize<TaxiDriverAuditEvent>();
					var payload = auditEvent.AuditPayload;
					var basePath = $"{auditEvent.AuthorityId}/auditObjects";
					var writeItem = payload.Audit;
					var userContext = executionContext.GetService<ITaxiUserContext>();
					var fileStorage = executionContext.GetService<IFileStorage>();


					((ISettableUserContext)userContext).SetAuthorityId(auditEvent.AuthorityId);
					((ISettableUserContext)userContext).SetTenantId(auditEvent.TenantId);
					((ISettableUserContext)userContext).SetSmartPAUserId(auditEvent.UserId);
					((ISettableUserContext)userContext).SetDisplayName(auditEvent.DisplayName);

					if (writeItem.OperationType.Equals(OperationTypes.Changing))
					{
						var jsonFile = payload.Data.JsonSerialize(false);
						var fileName = $"{writeItem.ItemId}_{DateTime.UtcNow:ddMMyyyyHHmmss}.json";

						writeItem.OldPathItem = writeItem.ItemType switch
						{
							ItemTypes.Licensee => basePath += $"/licensees/{fileName}",
							ItemTypes.Owner => basePath += $"/owners/{fileName}",
							ItemTypes.Driver => basePath += $"/drivers/{fileName}",
							ItemTypes.Vehicle => basePath += $"/vehicles/{fileName}",
							ItemTypes.TaxiDriverAssociation => basePath += $"/associations/{fileName}",
							ItemTypes.TaxiDriverSubstitution => basePath += $"/substitutions/{fileName}",
							_ => basePath
						};
						await fileStorage.UploadFileAsync(ApplicationsBlobStorages.Taxi, basePath,
																new MemoryStream(Encoding.UTF8.GetBytes(jsonFile)),
																MimeTypes.JsonFormat.Code, fileName);
					}

					var auditsService = executionContext.GetService<IAuditsService>();

					await auditsService.AddAudit(writeItem);

					break;
				default:
					break;
			}
		}
	}
}