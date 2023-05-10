using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.DataAccessLayer.EFCore.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
	[Table(Tables.Vehicles, Schema = Schemas.Taxi)]
	public class VehicleEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Required]
		public string? Model { get; set; }

		[Required]
		public string? LicensePlate { get; set; }

		[Required]
		public CarFuelTypes CarFuelType { get; set; }

		[NoUTC]
		[Required]
		public DateTime RegistrationDate { get; set; }

		[NoUTC]
		[Required]
		public DateTime InUseSince { get; set; }

		[Required]
		public long AuthorityId { get; set; }

		public long LicenseeId { get; set; }

		public DateTime SysStartTime { get; set; }

		[ForeignKey(nameof(LicenseeId))]
		public LicenseeEntity? Licensee { get; set; }
	}
}