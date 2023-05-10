namespace Asf.Taxi.BusinessLogic.Services
{
    public interface IExportService
    {
        Task<string> GetExcelExport(IEnumerable<object> objectToExport, string worksheetName);
    }
}