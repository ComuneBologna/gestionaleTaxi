using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Models
{
	public class TaxiDriverWrite
	{
		[Required]
		[Label(ResourcesConst.PersonId)]
		public long PersonId { get; set; }

		[RegularExpression("^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ResourcesConst.ShiftStartsFormat)]
		public string? ShiftStarts { get; set; }

		public FamilyCollaborationTypes? CollaborationType { get; set; }

		public List<Document> Documents { get; set; } = new List<Document>();
	}
}