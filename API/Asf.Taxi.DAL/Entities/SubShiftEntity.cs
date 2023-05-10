using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
	[Table(Tables.SubShifts, Schema = Schemas.Taxi)]
	public class SubShiftEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Required]
		public long AuthorityId { get; set; }

		[Required]
		public long ShiftId { get; set; }

		[Required]
		[MaxLength(256)]
		public string? Name { get; set; }

		[Required]
		public bool IsEnabled { get; set; }

		public DayOfWeek? RestDay { get; set; }

		[ForeignKey(nameof(ShiftId))]
		public ShiftEntity? Shift { get; set; }
	}
}