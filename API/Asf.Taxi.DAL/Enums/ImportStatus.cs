namespace Asf.Taxi.DAL.Enums
{
	public enum ImportStatus : byte
	{
		Uploaded = 1,
		InProgress,
		Imported,
		ImportedWithErrors,
	}
}