using Asf.Taxi.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
    [Table(Tables.LicenseesTaxiDrivers, Schema = Schemas.Taxi)]
    public class LicenseeTaxiDriverEntity
    {
        [Key]
        [Required]
        public long AuthorityId { get; set; }
        
        [Key]
        [Required]
        public long LicenseeId { get; set; }

        [Required]
        public LicenseeStatus LicenseeStatus { get; set; }

        [Key]
        [Required]
        public LicenseeTypes LicenseeType { get; set; }

        [Required]
        public bool IsFinancialAdministration { get; set; }

        [Key]
        [Required]
        public long TaxiDriverId { get; set; }

        [Key]
        [Required]
        public DriverTypes TaxiDriverType { get; set; }

        [ForeignKey(nameof(LicenseeId))]
        public LicenseeEntity? Licensee { get; set; }

        [ForeignKey(nameof(TaxiDriverId))]
        public TaxiDriverEntity? TaxiDriver { get; set; }
    }
}