using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
	[Table(Tables.Shifts, Schema = Schemas.Taxi)]
	public class ShiftEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Required]
		public long AuthorityId { get; set; }

		[Required]
		[MaxLength(256)]
		public string? Name { get; set; }

		[Required]
		public byte DurationInHour { get; set; }

		[Required]
		public bool IsHandicapMode { get; set; }

		public byte? HandicapBeforeInHour { get; set; }

		public byte? HandicapAfterInHour { get; set; }

		[Required]
		public byte BreakInHours { get; set; }

		[Required]
		public byte BreakThresholdActivationInHour { get; set; }

		[Required]
		public byte RestDayFrequencyInDays { get; set; }

		[Required]
		public bool IsEnabled { get; set; }

		public ICollection<SubShiftEntity> SubShifts { get; set; } = new List<SubShiftEntity>();
	}
}