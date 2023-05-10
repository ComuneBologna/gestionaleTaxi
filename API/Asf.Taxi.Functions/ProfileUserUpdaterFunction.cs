using Asf.Taxi.DAL;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartTech.Common.Models;
using SmartTech.Common.Services;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Functions;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using SmartTech.Common;
using SmartTech.Infrastructure.Caching;
using SmartTech.Common.ApplicationUsers.Events;
using SmartTech.Common.ApplicationUsers.Functions;

namespace Asf.Taxi.Functions
{
	public static class ProfileUserUpdaterFunction
	{
		[Function("ProfileUserUpdaterFunction")]
		public static async Task Run([ServiceBusTrigger(ApplicationsTopics.SmartPAEventsName, SubscriptionNames.TaxiDriverProfileUpdater, Connection = "servicebusconnection")] SmartPASystemEvent smartPAevent, FunctionContext context)
		{
			var logger = context.GetLogger("ProfileUserUpdaterFunction");

			logger.LogInformation($"C# ServiceBus topic trigger function processed message: {smartPAevent.JsonSerialize()}");
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("it-IT");

			var config = context.GetService<IConfiguration>();
			var cacheManager = context.GetService<ICacheManager>();
			var smartPAClientDataOptions = context.GetService<IOptions<SmartPAClientData>>();
			var httpClientFactory = context.GetService<IHttpClientFactory>();
			var dbContext = context.GetService<TaxiDriverDBContext>();
			var backofficeUsersService = context.GetService<IBackofficeUsersService>();

			try
			{
				await dbContext.ManageUser(config, cacheManager, httpClientFactory, backofficeUsersService, smartPAevent, smartPAClientDataOptions);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Internal error in ProfileUserUpdaterFunction: {ex.Message}");
				Console.WriteLine($"Internal error in ProfileUserUpdaterFunction [{DateTime.UtcNow}]: {ex.Message}");
			}
		}
	}
}