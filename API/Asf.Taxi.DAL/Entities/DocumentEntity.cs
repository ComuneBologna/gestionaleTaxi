using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.DataAccessLayer.EFCore.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
    [Table(Tables.Documents, Schema = Schemas.Taxi)]
    public class DocumentEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long AuthorityId { get; set; }

        [Required]
        public DocumentTypes Type { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string ReleasedBy { get; set; }

        [NoUTC]
        [Required]
        public DateTime ValidityDate { get; set; }

        [Required]
        public long TaxiDriverId { get; set; }

        [ForeignKey(nameof(TaxiDriverId))]
        public TaxiDriverEntity? TaxiDriver { get; set; }

    }
}