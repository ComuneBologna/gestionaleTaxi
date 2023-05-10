using Asf.Taxi.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
    [Table(Tables.RequestsRegisters, Schema = Schemas.Taxi)]
    public class RequestsRegisterEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long AuthorityId { get; set; }

        [Required]
        public long LicenseeId { get; set; }

        [Required]
        public long TemplateId { get; set; }

        [Required]
        public string? DMSDocumentId { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }

        [Required]
        public ExecutiveDigitalSignStatus ExecutiveDigitalSignStatus { get; set; }

        public string? DigitalSignResult { get; set; }

        [Required]
        public Guid AuthorUserId { get; set; }

        [ForeignKey(nameof(LicenseeId))]
        public LicenseeEntity? Licensee { get; set; }

        [ForeignKey(nameof(TemplateId))]
        public TemplateEntity? Template { get; set; }
    }
}