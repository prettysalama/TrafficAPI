namespace TrafficAPI.Models
{
    public class Car
    {
        public string   PlateNumber           { get; set; }
        public string   Model                 { get; set; }
        public string   Color                 { get; set; }
        public int      Year                  { get; set; }
        public int      Cylinders             { get; set; }
        public int      MotorCapacity         { get; set; }
        public DateTime CheckingDate          { get; set; }
        public DateTime LicenseCreationDate   { get; set; }
        public DateTime LicenseExpirationDate { get; set; }
        public string   OwnerName             { get; set; }
    }
}
