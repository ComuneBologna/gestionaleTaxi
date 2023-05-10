using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
    [Table(Tables.Emails, Schema = Schemas.Protocol)]
    public class EmailEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long AuthorityId { get; set; }

        [Required]
        [MaxLength(512)]
        public string? Email { get; set; }

        [MaxLength(1024)]
        public string? Description { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
