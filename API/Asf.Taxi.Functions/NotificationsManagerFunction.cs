using Asf.Taxi.BusinessLogic.Events;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartTech.Common;
using SmartTech.Infrastructure.Email;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Functions;
using SmartTech.Infrastructure.SystemEvents;
using System.Globalization;

namespace Asf.Taxi.Functions
{
    public static class NotificationsManagerFunction
	{
		readonly static string _emailBody = "Sono arrivati nuovi documenti da firmare. Accedere al sistema Taxi per firmarli.";
		readonly static string _emailSubject = "FIRMA DIGITALE DOCUMENTI TAXI";
		readonly static List<string> allowedTypes = new()
		{
			TaxiDriverEventTypes.RequestToSign
		};

		[Function("NotificationsManagerFunction")]
		public static async Task Run([ServiceBusTrigger(ApplicationsTopics.TaxiDriverEventsName, SubscriptionNames.TaxiDriverRequests, Connection = "servicebusconnection")] SystemEventBase sysEvent, FunctionContext executionContext)
		{
			var logger = executionContext.GetLogger("NotificationsManagerFunction");

			logger.LogInformation($"C# ServiceBus topic trigger function processed message: {sysEvent}");
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("it-IT");

			if (allowedTypes.Contains(sysEvent?.Type ?? string.Empty))
			{
				try
				{
					var emailService = executionContext.GetService<IEmailService>();

					if (sysEvent.Type.Equals(TaxiDriverEventTypes.RequestToSign))
					{
						var payload = sysEvent.Payload.JsonDeserialize<RequestToSignEvent>();

                        await emailService.SendEmailAsync(new()
                        {
                            Message = _emailBody,
                            Recipients = new List<string>()
                                {
                                    payload.ExecutiveEmail
                                },
                            Subject = _emailSubject
                        });
                    }
				}
				catch (Exception ex)
				{
					logger.LogError(ex, $"Internal error in NotificationsManagerFunction: {ex.Message}");
					Console.WriteLine($"Internal error in NotificationsManagerFunction [{DateTime.UtcNow}]: {ex.Message}");
				}
			}
		}
	}
}