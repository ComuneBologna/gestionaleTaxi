using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.DataAccessLayer.EFCore.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
    [Table(Tables.TaxiDriverSubstitutions, Schema = Schemas.Taxi)]
    public class TaxiDriverSubstitutionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long AuthorityId { get; set; }

        [Required]
        public long LicenseeId { get; set; }

        [Required]
        public long DriverToId { get; set; }

        [NoUTC]
        [Required]
        public DateTime StartDate { get; set; }

        [NoUTC]
        [Required]
        public DateTime EndDate { get; set; }

        public string? Note { get; set; }

        [Required]
        public SubstitutionStatus? Status { get; set; }

        [ForeignKey(nameof(DriverToId))]
        public TaxiDriverEntity? DriverTo { get; set; }

        [ForeignKey(nameof(LicenseeId))]
        public LicenseeEntity? Licensee { get; set; }
    }
}