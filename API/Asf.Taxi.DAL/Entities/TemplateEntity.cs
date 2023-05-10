using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
    [Table(Tables.Templates, Schema = Schemas.Taxi)]
    public class TemplateEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long AuthorityId { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? FileName { get; set; }

        public string? MimeType { get; set; }

        [Required]
        public string? FileId { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }

        [Required]
        public bool Deleted { get; set; }
    }
}