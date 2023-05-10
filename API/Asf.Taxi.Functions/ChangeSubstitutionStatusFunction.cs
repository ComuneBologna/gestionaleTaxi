using Asf.Taxi.BusinessLogic.Extensions;
using Asf.Taxi.DAL;
using Asf.Taxi.DAL.Entities;
using Asf.Taxi.DAL.Enums;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Functions;

namespace Asf.Taxi.Functions
{
    public static class ChangeSubstitutionStatusFunction
    {
        [Function("ChangeSubstitutionStatusFunction")]
        public static async Task ChangeSubstitutionStatusRun([TimerTrigger("0 0 3 * * *")] TimerInfo timer, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("ChangeSubstitutionStatusFunction");
            var dbContext = executionContext.GetService<TaxiDriverDBContext>();

            logger.LogInformation($"C# Timer trigger function executed at: {timer.ScheduleStatus.JsonSerialize()}");

            var substitutionsToCheck = await dbContext.DriverSubstitutions
                .AsNoTracking()
                .Where(ds => ds.Status != SubstitutionStatus.Archived)
                .ToListAsync();

            List<TaxiDriverSubstitutionEntity> substitutionsToChange = new();

            foreach (var item in substitutionsToCheck)
            {
                var newStatus = DateTime.UtcNow.Date.CalculateSubstitutionStatus(item.StartDate.Date, item.EndDate.Date);
                if (item.Status != newStatus)
                {
                    item.Status = newStatus;
                    substitutionsToChange.Add(item);
                }
            }
            foreach (var chunck in substitutionsToChange.ChunkBy(200))
            {
                dbContext.DriverSubstitutions.UpdateRange(chunck);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
