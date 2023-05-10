using SmartTech.Infrastructure.DataAccessLayer.EFCore.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
	[Table(Tables.CalendarShifts, Schema = Schemas.Taxi)]
	public class CalendarShiftEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[NoUTC]
		[Required]
		public DateTime Date { get; set; }

		public bool IsRestDay { get; set; }

		public bool IsHoliday { get; set; }  //FERIE PERSONALI

		public bool IsSwitch { get; set; }

		[NoUTC]
		public DateTime? DateSwitched { get; set; }

		public bool IsAllarmAccepted { get; set; }

		public bool IsVehicleStop { get; set; }

		public bool IsSickness { get; set; }

		[Required]
		public long LicenseeId { get; set; }

		[Required]
		public long AuthorityId { get; set; }

		[ForeignKey(nameof(LicenseeId))]
		public LicenseeEntity? Licensee { get; set; }
	}
}