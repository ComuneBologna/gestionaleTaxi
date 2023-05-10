using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.DataAccessLayer.EFCore.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
	[Table(Tables.Licensees, Schema = Schemas.Taxi)]
	public class LicenseeEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Required]
		public long AuthorityId { get; set; }

		[Required]
		[MaxLength(64)]
		public string? Number { get; set; }

		[NoUTC]
		[Required]
		public DateTime ReleaseDate { get; set; }

		[NoUTC]
		[Required]
		public DateTime EndDate { get; set; }

		public string? ExpireActivityCause { get; set; }

		public long? ShiftId { get; set; }

		public long? SubShiftId { get; set; }

		public long? TaxiDriverAssociationId { get; set; }

		public long? LicenseesIssuingOfficeId { get; set; }

		public string? Note { get; set; }

		public string? Acronym { get; set; }

		[Required]
		public bool IsFinancialAdministration { get; set; }

		[Required]
		public bool IsFamilyCollaboration { get; set; }

		public string? GarageAddress { get; set; }

		[Required]
		public LicenseeStatus Status { get; set; }

		[Required]
		public LicenseeTypes Type { get; set; }

		public DateTime SysStartTime { get; set; }

		public VehicleEntity? Vehicle { get; set; }

		public FinancialAdministrationEntity? FinancialAdministration { get; set; }

		public Guid? FolderId { get; set; }

		[ForeignKey(nameof(LicenseesIssuingOfficeId))]
		public LicenseesIssuingOfficeEntity? LicenseesIssuingOffice { get; set; }

		[ForeignKey(nameof(ShiftId))]
		public ShiftEntity? Shift { get; set; }

		[ForeignKey(nameof(SubShiftId))]
		public SubShiftEntity? SubShift { get; set; }

		[ForeignKey(nameof(TaxiDriverAssociationId))]
		public TaxiDriverAssociationEntity? TaxiDriverAssociation { get; set; }

		public ICollection<CalendarShiftEntity> CalendarShifts { get; set; } = new List<CalendarShiftEntity>();

		public ICollection<LicenseeTaxiDriverEntity> LicenseesTaxiDrivers { get; set; } = new List<LicenseeTaxiDriverEntity>();

		public ICollection<TaxiDriverSubstitutionEntity> TaxiDriverSubstitutions { get; set; } = new List<TaxiDriverSubstitutionEntity>();
	}
}