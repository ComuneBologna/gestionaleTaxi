using Asf.Taxi.DAL;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartTech.Common.Models;
using SmartTech.Common.Services;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Functions;
using System.Globalization;
using SmartTech.Common;
using SmartTech.Infrastructure.SystemEvents;
using Asf.Taxi.BusinessLogic.Events;
using SmartTech.Infrastructure.Storage;
using SmartTech.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using SmartTech.Infrastructure.Exceptions;
using Asf.Taxi.BusinessLogic.Extensions;

namespace Asf.Taxi.Functions
{
	public static class FolderCreatorFunction
	{
		[Function("FolderCreatorFunction")]
		public static async Task Run([ServiceBusTrigger(ApplicationsTopics.TaxiDriverEventsName, SubscriptionNames.TaxiDriverCreateFolder, Connection = "servicebusconnection")] SystemEventBase sysEvent, FunctionContext executionContext)
		{
			var type = sysEvent.Type ?? string.Empty;
			var logger = executionContext.GetLogger("VariationStoreFunction");

			logger.LogInformation($"C# ServiceBus topic trigger function processed message: {sysEvent.JsonSerialize()}");
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("it-IT");

			if (type != TaxiDriverEventTypes.CreateFolder)
				return;

			try
			{
				var fileStorage = executionContext.GetService<IFileStorage>();
				var dbContext = executionContext.GetService<TaxiDriverDBContext>();
				var dms = executionContext.GetService<IDocumentManagementSystemService>();
				var tae = sysEvent.Payload.JsonDeserialize<TaxiDriverFolderCreateEvent>();
				var authorityId = tae.AuthorityId;
				var smartPAClientDataOptions = executionContext.GetService<IOptions<SmartPAClientData>>();
				var httpClientFactory = executionContext.GetService<IHttpClientFactory>();
				var accessToken = await httpClientFactory.GetClientApplicationToken(smartPAClientDataOptions.Value, authorityId.ToString(), tae.TenantId);
				var licenseeEntity = await dbContext.Licensees.FirstOrDefaultAsync(l => l.Id == tae.LicenseeId) ??
					throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);

				if (licenseeEntity.FolderId.HasValue)
					return;

				await licenseeEntity.CreateFolder(fileStorage, dms, tae.AuthorityId, tae.UserId, tae.RolePath, tae.LicenseeNumber, tae.LicenseeType, accessToken);
				dbContext.Licensees.Update(licenseeEntity);
				await dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Internal error in FolderCreatorFunction: {ex.Message}");
				Console.WriteLine($"Internal error in FolderCreatorFunction [{DateTime.UtcNow}]: {ex.Message}");
			}
		}
	}
}