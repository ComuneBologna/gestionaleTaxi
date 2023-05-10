using Asf.Taxi.BusinessLogic;
using Asf.Taxi.BusinessLogic.Extensions;
using Asf.Taxi.Functions.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartTech.Common.Extensions;
using SmartTech.Infrastructure.Configuration.AzureApp;
using SmartTech.Infrastructure.Logging;
using SmartTech.Infrastructure.Storage.AzureStorage;
using SmartTech.Infrastructure.Cache.Redis;
using SmartTech.Infrastructure.Email.Sendgrid;

namespace Asf.Taxi.Functions
{
	public class Program
	{
		public static void Main()
		{
			var host = new HostBuilder()
			.ConfigureAppConfiguration(builder =>
			{
				builder.ConfigureAzureAppConfiguration("TaxiDriver");
				builder.AddEnvironmentVariables();
			})
			.ConfigureFunctionsWorkerDefaults()
			.ConfigureServices((builder, services) =>
			{
				var config = builder.Configuration;

				services.AddHttpClient();
				services.AddDependencyTracker();
				services.AddApplicationInsightsForConsole(config, "taxidriverfunctions");
				services.AddCustomLogging(config);
				services.AddSmartPAServices(config);
				services.AddTaxiServices(config);
				services.AddSingleton(config);
				services.AddScoped<ITaxiUserContext, UserContext>();
				services.AddScoped<ISettableUserContext, UserContext>();
				services.AddFileStorage(options => options.Endpoint = Environment.GetEnvironmentVariable("BlobStorageEndpoint") ??
																		config.GetSection("BlobStorage:Endpoint")?.Value);
				services.AddEmailServices(options =>
				{
					options.ApiKey = config["SendGridMailer:ApiKey"];
					options.SenderAddress = config["SendGridMailer:SenderAddress"];
					options.SenderName = config["SendGridMailer:SenderName"];
				});

				services.AddCaching(options =>
				{
					options.CacheConfiguration = config["Cache:CacheConfiguration"];
					options.MaxObjectSize = int.Parse(config["Cache:MaxObjectSize"]);
					options.ThrowExceptionOnError = bool.Parse(config["Cache:ThrowExceptionOnError"]);
					options.ThrowExceptionOnMiss = bool.Parse(config["Cache:ThrowExceptionOnMiss"]);
				});
			}).Build();

			host.Run();
		}
	}
}