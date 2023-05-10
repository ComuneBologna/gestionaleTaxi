using Asf.Taxi.DAL.Enums;
using SmartTech.Common.Enums;
using SmartTech.Infrastructure.DataAccessLayer.EFCore.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
    [Table(Tables.TaxiDrivers, Schema = Schemas.Taxi)]
    public class TaxiDriverEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long AuthorityId { get; set; }

        [MaxLength(128)]
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [MaxLength(64)]
        public string? FiscalCode { get; set; }

        [NoUTC]
        public DateTime? BirthDate { get; set; }

        [MaxLength(128)]
        public string? BirthCity { get; set; }

        [MaxLength(128)]
        public string? BirthProvince { get; set; }

        [MaxLength(128)]
        public string? ResidentCity { get; set; }

        [MaxLength(128)]
        public string? ResidentProvince { get; set; }

        [MaxLength(16)]
        public string? PhoneNumber { get; set; }

        [MaxLength(256)]
        public string? EmailOrPEC { get; set; }

        public string? Address { get; set; }

        [MaxLength(16)]
        public string? ZipCode { get; set; }

        [NotMapped]
        public string DisplayName => $"{LastName} {FirstName}".Trim();

        [NotMapped]
        public string ExtendedDisplayName => $"{$"{LastName} {FirstName}".Trim()}, {FiscalCode}";

        public DateTime SysStartTime { get; set; }

        public byte? ShiftStartHour { get; set; }

        public byte? ShiftStartMinutes { get; set; }

        public PersonType? Type { get; set; }

        public FamilyCollaborationTypes? CollaborationType { get; set; }

        public ICollection<TaxiDriverSubstitutionEntity> Substitutions { get; set; } = new List<TaxiDriverSubstitutionEntity>();

        public ICollection<DocumentEntity> Documents { get; set; } = new List<DocumentEntity>();

        public ICollection<LicenseeTaxiDriverEntity> LicenseesTaxiDrivers { get; set; } = new List<LicenseeTaxiDriverEntity>();
    }
}