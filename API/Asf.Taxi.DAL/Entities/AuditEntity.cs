using Asf.Taxi.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asf.Taxi.DAL.Entities
{
	[Table(Tables.Audits, Schema = Schemas.Taxi)]
	public class AuditEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Required]
		public long ItemId { get; set; }

		[Required]
		public ItemTypes ItemType { get; set; }

		[Required]
		public long AuthorityId { get; set; }

		[Required]
		public Guid UserId { get; set; }

		[Required]
		public string? Username { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; }

		[Required]
		public OperationTypes OperationType { get; set; }

		public string? OldItemPath { get; set; }

		public string? MemoLine { get; set; }
	}
}