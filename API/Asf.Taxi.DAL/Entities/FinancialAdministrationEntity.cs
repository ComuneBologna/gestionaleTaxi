using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
    [Table(Tables.FinancialAdministrations, Schema = Schemas.Taxi)]
    public class FinancialAdministrationEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long AuthorityId { get; set; }

        [Required]
        public long LicenseeId { get; set; }

        [Required]
        public long LegalPersonId { get; set; }

        [ForeignKey(nameof(LicenseeId))]
        public LicenseeEntity? Licensee { get; set; }

        [ForeignKey(nameof(LegalPersonId))]
        public TaxiDriverEntity? LegalPerson { get; set; }

        public bool Deleted { get; set; }
    }
}