using Asf.Taxi.API.Models;
using Asf.Taxi.BusinessLogic.Models;

namespace Asf.Taxi.API.Mappers
{
    public static class PeopleMapper
    {
        public static PhysicalPersonWrite MapPhysical(this PersonWriteAPI person)
            => new()
            {
                Address = person.Address,
                BirthCity = person.BirthCity,
                BirthDate = person.BirthDate,
                BirthProvince = person.BirthProvince,
                EmailOrPEC = person.EmailOrPEC,
                FirstName = person.FirstName,
                FiscalCode = person.FiscalCode,
                LastName = person.LastName,
                PhoneNumber = person.PhoneNumber,
                ResidentCity = person.ResidentCity,
                ResidentProvince = person.ResidentProvince,
                ZipCode = person.ZipCode
            };

        public static LegalPersonWrite MapLegal(this PersonWriteAPI person)
            => new()
            {
                Address = person.Address,
                EmailOrPEC = person.EmailOrPEC,
                Nominative = person.LastName,
                PhoneNumber = person.PhoneNumber,
                VATNumber = person.FiscalCode
            };
    }
}
