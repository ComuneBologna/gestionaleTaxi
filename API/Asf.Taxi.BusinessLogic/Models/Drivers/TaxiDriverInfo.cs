using SmartTech.Common.Enums;

namespace Asf.Taxi.BusinessLogic.Models
{
    public class TaxiDriverInfo : TaxiDriverWrite
    {
        public long Id { get; set; }

        public string? PersonDisplayName { get; set; }

        public string? ExtendedPersonDisplayName { get; set; }

        public DateTime StartDate { get; set; }

        public string? Note { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? FiscalCode { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? BirthCity { get; set; }

        public string? BirthProvince { get; set; }

        public string? PhoneNumber { get; set; }

        public string? EmailOrPEC { get; set; }

        public string? Address { get; set; }

        public string? ZipCode { get; set; }

        public string? ResidentCity { get; set; }

        public string? ResidentProvince { get; set; }

        public bool? AllDocuments { get; set; }

        public PersonType? Type { get; set; }
    }
}