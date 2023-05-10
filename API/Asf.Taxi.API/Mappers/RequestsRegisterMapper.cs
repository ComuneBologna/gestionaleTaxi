using Asf.Taxi.API.Models;
using Asf.Taxi.BusinessLogic.Models.Requests;
using Asf.Taxi.DAL.Enums;

namespace Asf.Taxi.API.Mappers
{
    public static class RequestsRegisterMapper
    {
        public static RequestsRegisterFilterCriteria Map(this RequestsRegisterFilterCriteriaAPI criteria) =>
            new()
            {
                Ascending = criteria.Ascending,
                Description = criteria.Description,
                ExecutiveDigitalSignStatus = ExecutiveDigitalSignStatus.Required,
                Id = criteria.Id,
                ItemsPerPage = criteria.ItemsPerPage,
                KeySelector = criteria.KeySelector
            };
    }
}