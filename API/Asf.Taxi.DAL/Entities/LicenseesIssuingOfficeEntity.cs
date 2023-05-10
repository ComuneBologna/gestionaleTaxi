using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
    [Table(Tables.LicenseesIssuingOffices, Schema = Schemas.Taxi)]
    public class LicenseesIssuingOfficeEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public long AuthorityId { get; set; }

        public bool Deleted { get; set; }
    }
}