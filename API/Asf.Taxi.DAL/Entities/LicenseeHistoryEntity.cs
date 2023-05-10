using Asf.Taxi.DAL.Enums;
using SmartTech.Common.Enums;
using SmartTech.Infrastructure.DataAccessLayer.EFCore.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
    [Table(Tables.Licensees, Schema = Schemas.Variation)]
    public class LicenseeHistoryEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long AuthorityId { get; set; }

        public VariationTypes? VariationType { get; set; }

        [Required]
        public DateTime SysStartTime { get; set; }

        [Required]
        public DateTime SysEndTime { get; set; }

        public string? Note { get; set; }

        [Required]
        public long LicenseeId { get; set; }

        [MaxLength(64)]
        public string? Number { get; set; }

        [NoUTC]
        public DateTime? ReleaseDate { get; set; }

        [NoUTC]
        public DateTime? EndDate { get; set; }

        public string? ExpireActivityCause { get; set; }

        public long? ShiftId { get; set; }

        public long? SubShiftId { get; set; }

        public long? LicenseesIssuingOfficeId { get; set; }

        public string? LicenseesIssuingOfficeDescription { get; set; }

        public string? LicenseeNote { get; set; }

        public string? Acronym { get; set; }

        public bool? IsFinancialAdministration { get; set; }

        public bool? IsFamilyCollaboration { get; set; }

        public string? GarageAddress { get; set; }

        public LicenseeStatus? Status { get; set; }

        public LicenseeTypes? Type { get; set; }

        public long? FinancialAdministrationId { get; set; }

        public long? TaxiDriverAssociationId { get; set; }

        public string? TaxiDriverAssociationName { get; set; }

        public string? TaxiDriverAssociationFiscalCode { get; set; }

        public long? VehicleId { get; set; }

        public string? VehicleModel { get; set; }

        public string? VehicleLicensePlate { get; set; }

        public CarFuelTypes? VehicleCarFuelType { get; set; }

        [NoUTC]
        public DateTime? VehicleRegistrationDate { get; set; }

        [NoUTC]
        public DateTime? VehicleInUseSince { get; set; }

        public long? TaxiDriverId { get; set; }

        [MaxLength(128)]
        public string? TaxiDriverFirstName { get; set; }

        public string? TaxiDriverLastName { get; set; }

        [MaxLength(64)]
        public string? TaxiDriverFiscalCode { get; set; }

        public DriverTypes? TaxiDriverType { get; set; }

        public PersonType? TaxiDriverPersonType { get; set; }

        public byte? TaxiDriverShiftStartHour { get; set; }

        public byte? TaxiDriverShiftStartMinutes { get; set; }

        public FamilyCollaborationTypes? TaxiDriverCollaborationType { get; set; }

        public Guid? FolderId { get; set; }
    }
}