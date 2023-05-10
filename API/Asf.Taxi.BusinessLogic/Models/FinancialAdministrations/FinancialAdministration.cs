namespace Asf.Taxi.BusinessLogic.Models.FinancialAdministrations
{
    public class FinancialAdministration : FinancialAdministrationWrite
    {
        public long Id { get; set; }

        public string? LegalPersonDisplayName { get; set; }
    }
}