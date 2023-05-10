namespace Asf.Taxi.BusinessLogic.Extensions
{
	public static class TagsConstants
	{
		public const string DateNow = "Data odierna";
		public const string InternalProtocol = "Protocollazione interna";
		public const string Date = "Data";
		public const string Year = "Anno";
		public const string Day = "Giorno";
		public const string Months = "N° mesi";
		public const string DateFrom = "Dalla data";
		public const string DateTo = "Alla data";
		public const string ProtocolNumber = "N° protocollo";
		public const string ProtocolDate = "Data protocollo"; 
		//public const string InternalProtocolNumber = "N° protocollo interno";
		//public const string InternalProtocolDate = "Data protocollo interno";
		public const string Note = "Note";
		public const string CollaboratorRelationship = "Grado di parentela";
		public const string FreeText = "Testo libero";
		public const string LicenseeNumber = "Numero licenza";
		public const string LicenseeReleaseDate = "Data rilascio licenza";
		public const string LicenseeIssuingOffice = "Ente di rilascio licenza";
		public const string LicenseeType = "Tipo licenza";
		public const string VehiclePlate = "Targa veicolo attuale";
		public const string VehicleModel = "Modello veicolo attuale";
		public const string SubstitutedVehiclePlate = "Targa veicolo sostituito";
		public const string SubstitutedVehicleModel = "Modello veicolo sostituito";
		public const string OwnerDisplayName = "Nome e cognome proprietario";
		public const string OwnerFiscalCode = "Codice fiscale proprietario";
		public const string OwnerBirthCity = "Città di nascita proprietario";
		public const string OwnerBirthProvince = "Provincia di nascita proprietario";
		public const string OwnerBirthDate = "Data di nascita proprietario";
		public const string OwnerResidentCity = "Città di residenza proprietario";
		public const string OwnerResidentAddress = "Indirizzo di residenza proprietario";
		public const string OwnerResidentProvince = "Provincia di residenza proprietario";
		public const string CollaboratorDisplayName = "Nome e cognome collaboratore";
		public const string CollaboratorFiscalCode = "Codice fiscale collaboratore";
		public const string CollaboratorBirthCity = "Città di nascita collaboratore";
		public const string CollaboratorBirthProvince = "Provincia di nascita collaboratore";
		public const string CollaboratorBirthDate = "Data di nascita collaboratore";
		public const string CollaboratorResidentCity = "Città di residenza collaboratore";
		public const string CollaboratorResidentAddress = "Indirizzo di residenza collaboratore";
		public const string CollaboratorResidentProvince = "Provincia di residenza collaboratore";
		public const string SubstitutedDisplayName = "Nome e cognome sostituto";
		public const string SubstitutedFiscalCode = "Codice fiscale sostituto";
		public const string SubstitutedBirthCity = "Città di nascita sostituto";
		public const string SubstitutedBirthProvince = "Provincia di nascita sostituto";
		public const string SubstitutedBirthDate = "Data di nascita sostituto";
		public const string SubstitutedResidentCity = "Città di residenza sostituto";
		public const string SubstitutedResidentAddress = "Indirizzo di residenza sostituto";
		public const string SubstitutedResidentProvince = "Provincia di residenza sostituto";

		public static Dictionary<string, string> Tags { get; set; } = new()
		{
			{ DateNow, "{DateNow}" },
			{ InternalProtocol, "{InternalProtocol}" },
			{ Date, "{AdditionalInformation.Date}" },
			{ Year, "{AdditionalInformation.Year}" },
			{ Day, "{AdditionalInformation.Day}" },
			{ Months, "{AdditionalInformation.Months}" },
			{ DateFrom, "{AdditionalInformation.DateFrom}" },
			{ DateTo, "{AdditionalInformation.DateTo}" },
			{ ProtocolNumber, "{AdditionalInformation.ProtocolNumber}" },
			{ ProtocolDate, "{AdditionalInformation.ProtocolDate}" },
			//{ InternalProtocolNumber, "{AdditionalInformation.InternalProtocolNumber}" },
			//{ InternalProtocolDate, "{AdditionalInformation.InternalProtocolDate}" },
			{ Note, "{AdditionalInformation.Note}" },
			{ CollaboratorRelationship, "{AdditionalInformation.CollaboratorRelationship}" },
			{ FreeText, "{AdditionalInformation.FreeText}" },
			{ LicenseeNumber, "{Licensee.Number}" },
			{ LicenseeReleaseDate, "{Licensee.ReleaseDate}" },
			{ LicenseeIssuingOffice, "{Licensee.LicenseeIssuingOffice}" },
			{ LicenseeType, "{Licensee.Type}" },
			{ VehiclePlate, "{Licensee.VehiclePlate}" },
			{ VehicleModel, "{Licensee.VehicleModel}" },
			{ SubstitutedVehiclePlate, "{Licensee.SubstitutedVehiclePlate}" },
			{ SubstitutedVehicleModel, "{Licensee.SubstitutedVehicleModel}" },
			{ OwnerDisplayName, "{Owner.DisplayName}" },
			{ OwnerFiscalCode, "{Owner.FiscalCode}" },
			{ OwnerBirthCity, "{Owner.BirthCity}" },
			{ OwnerBirthProvince, "{Owner.BirthProvince}" },
			{ OwnerBirthDate, "{Owner.BirthDate}" },
			{ OwnerResidentCity, "{Owner.ResidentCity}" },
			{ OwnerResidentAddress, "{Owner.ResidentAddress}" },
			{ OwnerResidentProvince, "{Owner.ResidentProvince}" },
			{ CollaboratorDisplayName, "{Collaborator.DisplayName}" },
			{ CollaboratorFiscalCode, "{Collaborator.FiscalCode}" },
			{ CollaboratorBirthCity, "{Collaborator.BirthCity}" },
			{ CollaboratorBirthProvince, "{Collaborator.BirthProvince}" },
			{ CollaboratorBirthDate, "{Collaborator.BirthDate}" },
			{ CollaboratorResidentCity, "{Collaborator.ResidentCity}" },
			{ CollaboratorResidentAddress, "{Collaborator.ResidentAddress}" },
			{ CollaboratorResidentProvince, "{Collaborator.ResidentProvince}" },
			{ SubstitutedDisplayName, "{Substituted.DisplayName}" },
			{ SubstitutedFiscalCode, "{Substituted.FiscalCode}" },
			{ SubstitutedBirthCity, "{Substituted.BirthCity}" },
			{ SubstitutedBirthProvince, "{Substituted.BirthProvince}" },
			{ SubstitutedBirthDate, "{Substituted.BirthDate}" },
			{ SubstitutedResidentCity, "{Substituted.ResidentCity}" },
			{ SubstitutedResidentAddress, "{Substituted.ResidentAddress}" },
			{ SubstitutedResidentProvince, "{Substituted.ResidentProvince}" }
		};
	}
}