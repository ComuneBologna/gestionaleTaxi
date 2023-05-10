namespace Asf.Taxi.BusinessLogic.Models.Licensees
{
    public class LicenseeTaxiWrite : LicenseeWrite
    {
        public long? ShiftId { get; set; }

        public long? SubShiftId { get; set; }
    }
}