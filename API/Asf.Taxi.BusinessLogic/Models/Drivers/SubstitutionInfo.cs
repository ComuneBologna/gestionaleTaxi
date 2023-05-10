using Asf.Taxi.DAL.Enums;

namespace Asf.Taxi.BusinessLogic.Models
{
    public class SubstitutionInfo : SubstitutionBase
    {
        public long Id { get; set; }

        public TaxiDriverSubstitution? SubstituteDriver { get; set; }

        public long SubstituteDriverId { get; set; }

        public SubstitutionStatus Status { get; set; }

        public bool IsExpiring { get; set; }
    }
}