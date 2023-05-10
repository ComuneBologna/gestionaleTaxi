using SmartTech.Infrastructure.DataAccessLayer.EFCore.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
    [Table(Tables.ProcessCodes, Schema = Schemas.Protocol)]
    public class ProcessCodeEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long AuthorityId { get; set; }

        [Required]
        public string? Code { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public bool Territorial { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? FullTextSearch { get; set; }
    }
}
