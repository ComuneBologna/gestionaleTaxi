using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
	[Table(Tables.TaxiDriverAssociations, Schema = Schemas.Taxi)]
	public class TaxiDriverAssociationEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		public string? Name { get; set; }

		public string? FiscalCode { get; set; }

		public string? Email { get; set; }

		public string? TelephoneNumber { get; set; }

		public ICollection<LicenseeEntity> Licensees { get; set; } = new List<LicenseeEntity>();

		[Required]
		public long AuthorityId { get; set; }

		[Required]
		public bool IsDeleted { get; set; }
	}
}