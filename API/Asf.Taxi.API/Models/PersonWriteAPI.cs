using Asf.Taxi.BusinessLogic.Localization;
using SmartTech.Common.Enums;
using SmartTech.Infrastructure.Validations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.API.Models
{
    public class PersonWriteAPI
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? FiscalCode { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? BirthCity { get; set; }

        public string? BirthProvince { get; set; }

        [RegularExpression(@"^[+]{0,1}([(][0-9]{1,4}[)]){0,1}[-\s./0-9]*$", ResourcesConst.PhoneNumberFormat)]
        public string? PhoneNumber { get; set; }

        [RegularExpression(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", ResourcesConst.EmailOrPECFormat)]
        public string? EmailOrPEC { get; set; }

        public string? Address { get; set; }

        [RegularExpression(@"^\d{5}$", ResourcesConst.ZipCodeFormat)]
        public string? ZipCode { get; set; }

        public string? ResidentCity { get; set; }

        public string? ResidentProvince { get; set; }
    }
}