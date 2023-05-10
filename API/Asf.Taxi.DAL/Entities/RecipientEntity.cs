using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
    [Table(Tables.Recipients, Schema = Schemas.Taxi)]
    public class RecipientEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long? AuthorityId { get; set; }

        [Required]
        public long? LicenseeId { get; set; }

        [Required]
        public string? Mail { get; set; }

        public byte? Order { get; set; } = 5;

        [ForeignKey(nameof(LicenseeId))]
        public LicenseeEntity? Licensee { get; set; }
    }
}