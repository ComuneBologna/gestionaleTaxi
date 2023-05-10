using SmartTech.Common.Enums;

namespace Asf.Taxi.BusinessLogic.Models
{
    public class PersonInfo : PersonBase, IReadPerson
    {
        public long Id { get; set; }

        public string? DisplayName { get; set; }

        public string? FiscalCode { get; set; }

        public PersonType? Type { get; set; }

        public string? ZipCode { get; set; }

        public string? ResidentCity { get; set; }

        public string? ResidentProvince { get; set; }
    }
}