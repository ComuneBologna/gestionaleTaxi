namespace Asf.Taxi.BusinessLogic.Models
{
    public class PersonAutocomplete : PersonAutocompleteBase
    {
        public List<Document> Documents { get; set; } = new();
    }
}