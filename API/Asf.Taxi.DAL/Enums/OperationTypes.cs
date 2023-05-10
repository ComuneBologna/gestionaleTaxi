namespace Asf.Taxi.DAL.Enums
{
	public enum OperationTypes : byte
	{
		/// <summary>
		/// Creazione elemento
		/// </summary>
		Creating = 1,

		/// <summary>
		/// Rettifica elemento
		/// </summary>
		Updating,

		/// <summary>
		/// Variazione elemento
		/// </summary>
		Changing,

		/// <summary>
		/// Cancellazione elemento
		/// </summary>
		Deleting
	}
}