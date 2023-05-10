namespace Asf.Taxi.BusinessLogic.Events
{
	public static class TaxiDriverEventTypes
	{
		public const string TaxiDriverAudit = "taxi:audit";
		//public const string TaxiDriverFileUploaded = "taxi:imports:fileuploaded";
		//public const string TaxiDriverStartImport = "taxi:imports:start";
		//public const string TaxiDriverEndImport = "taxi:imports:end";
		public const string TaxiDriverStartAssociation = "taxi:imports:startassociations";
		public const string RequestToSign = "Request:Sign";
		public const string CreateFolder = "dms:folder:create";
	}
}