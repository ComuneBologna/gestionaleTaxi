namespace Asf.Taxi.BusinessLogic.Models
{
    public class Person : PersonInfo
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? BirthCity { get; set; }

        public string? BirthProvince { get; set; }

        public string? PhoneNumber { get; set; }
    }
}