using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using TrafficAPI.Models;
using TrafficAPI.Requests;

namespace TrafficAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DrivingLicenseController : ControllerBase
    {
        private readonly IMongoCollection<Citizen> _citizens;

        public DrivingLicenseController(IMongoClient client)
        {
            var database = client.GetDatabase("driving-school");
            _citizens = database.GetCollection<Citizen>("citizens");
        }

        [HttpPost("login")]
        public ActionResult<Citizen> Login([FromBody] string id)
        {
            var citizen = _citizens.Find(c => c.Id == id).FirstOrDefault();
            if (citizen == null)
            {
                return NotFound();
            }
            return Ok(citizen);
        }

        [HttpPost("update-driving-license")]
        public ActionResult<Citizen> UpdateDrivingLicense([FromBody] DrivingLicenseUpdateRequest request)
        {
            var citizen = _citizens.Find(c => c.Id == request.CitizenId).FirstOrDefault();
            if (citizen == null)
            {
                return NotFound();
            }

            // Send request to the server to get license information based on citizen ID
            var licenseInfo = GetLicenseInformationFromServer(request.CitizenId);

            // Update citizen driving license information
            citizen.DrivingLicenseType = licenseInfo.DrivingLicenseType;
            citizen.LicenseIssuanceUnit = licenseInfo.LicenseIssuanceUnit;

            // Save changes to the database
            _citizens.ReplaceOne(c => c.Id == request.CitizenId, citizen);

            return Ok(citizen);
        }

        [HttpPost("schedule-driving-test")]
        public ActionResult<string> ScheduleDrivingTest([FromBody] DrivingTestRequest request)
        {
            var citizen = _citizens.Find(c => c.Id == request.CitizenId).FirstOrDefault();
            if (citizen == null)
            {
                return NotFound();
            }

            // Check if the requested day is available
            if (!IsDrivingTestDayAvailable(request.TestDay))
            {
                return BadRequest("The requested driving test day is not available.");
            }

            // Send request to the server to schedule the driving test
            var result = ScheduleDrivingTestOnServer(request);

            if (result == "Confirmed")
            {
                // Update citizen driving license information with the new driving test date and visa details
                citizen.DrivingTestDay = request.TestDay;
                citizen.VisaCard       = request.VisaCard;

                // Save changes to the database
                _citizens.ReplaceOne(c => c.Id == request.CitizenId, citizen);

                return Ok("Your driving test has been scheduled successfully.");
            }
            else
            {
                return BadRequest("Your driving test could not be scheduled at this time.");
            }
        }

        [HttpPost("issue-replacement-license")]
        public ActionResult<string> IssueReplacementLicense([FromBody] ReplacementLicenseRequest request)
        {
            var citizen = _citizens.Find(c => c.Id == request.CitizenId).FirstOrDefault();
            if (citizen == null)
            {
                return NotFound();
            }

            // Check if the citizen has a valid visa card
            if (!IsVisaCardValid(citizen.VisaCard, request.VisaCard))
            {
                return BadRequest("The visa card details are invalid.");
            }

            // Issue the replacement license
            return Ok("The replacement license has been issued successfully.");

        }

        private bool IsVisaCardValid(VisaCard storedVisaCard, VisaCard requestVisaCard)
        {
            // Check if the card numbers match
            if (storedVisaCard.CardNumber != requestVisaCard.CardNumber)
            {
                return false;
            }

            // Check if the expiration dates match
            if (storedVisaCard.ExpirationDate != requestVisaCard.ExpirationDate)
            {
                return false;
            }

            // Check if the CVV codes match
            if (storedVisaCard.CVV != requestVisaCard.CVV)
            {
                return false;
            }

            return true;
        }

        private LicenseInformation GetLicenseInformationFromServer(string id)
        {
            // In this example, we are assuming that the server has access to the necessary
            // driving license information and can return it as a LicenseInformation object
            return new LicenseInformation
            {
                DrivingLicenseType = "Class D",
                LicenseIssuanceUnit = "Cairo Unit"
            };
        }

        private bool IsDrivingTestDayAvailable(DateTime testDay)
        {
            // Check if the requested day is available for scheduling driving tests
            // (Assuming all weekdays are available except Friday and Saturday)
            return (testDay.DayOfWeek != DayOfWeek.Friday && testDay.DayOfWeek != DayOfWeek.Saturday);
        }

        private string ScheduleDrivingTestOnServer(DrivingTestRequest request)
        {
            // In this example, we are assuming that the server can always schedule the driving test
            // and return "Confirmed" as the result
            return "Confirmed";
        }

        [HttpGet("GenerateAndInsertCitizens")]
        public IActionResult GenerateAndInsertCitizens()
        {
            // Call the GenerateRandomCitizens method to generate 100 instances of the Citizen class
            var citizens = GenerateRandomCitizens(100);

            // Insert the citizens into the database
            _citizens.InsertMany(citizens);

            Console.WriteLine("Citizens have been generated and inserted into the database.");

            // Return the generated citizens
            return Ok(citizens);
        }

        static List<Citizen> GenerateRandomCitizens(int count)
        {
            var random   = new Random();
            var citizens = new List<Citizen>();

            for (int i = 0; i < count; i++)
            {
                var id                  = GenerateRandomId();
                var drivingLicenseType  = GenerateRandomDrivingLicenseType();
                var licenseIssuanceUnit = GenerateRandomLicenseIssuanceUnit();
                var drivingTestDay      = GenerateRandomDrivingTestDay();
                var visaCard            = GenerateRandomVisaCard();

                var citizen = new Citizen
                {
                    Id                  = id,
                    DrivingLicenseType  = drivingLicenseType,
                    LicenseIssuanceUnit = licenseIssuanceUnit,
                    DrivingTestDay      = drivingTestDay,
                    VisaCard            = visaCard
                };

                citizens.Add(citizen);
            }

            return citizens;
        }

        static VisaCard GenerateRandomVisaCard()
        {
            var random   = new Random();
            var visaCard = new VisaCard();

            // Generate a random 16-digit card number
            for (int i = 0; i < 16; i++)
            {
                visaCard.CardNumber += random.Next(0, 9);
            }

            // Generate a random expiration date between now and 5 years from now
            var expirationDate = DateTime.UtcNow.AddYears(random.Next(1, 6));
            visaCard.ExpirationDate = new DateTime(expirationDate.Year, expirationDate.Month, 1);

            // Generate a random 3-digit CVV
            for (int i = 0; i < 3; i++)
            {
                visaCard.CVV += random.Next(0, 9);
            }

            return visaCard;
        }

        static string GenerateRandomId()
        {
            var random = new Random();
            var id = "";
            for (int i = 0; i < 15; i++)
            {
                id += random.Next(0, 9);
            }
            return id;
        }

        static string GenerateRandomDrivingLicenseType()
        {
            var random = new Random();
            var types = new string[] { "Class A", "Class B", "Class C", "Class D", "Class E" };
            return types[random.Next(0, types.Length)];
        }

        static string GenerateRandomLicenseIssuanceUnit()
        {
            var random = new Random();
            var units = new string[] { "Cairo Unit", "Giza Unit", "Alexandria Unit", "Luxor Unit", "Aswan Unit" };
            return units[random.Next(0, units.Length)];
        }

        static DateTime GenerateRandomDrivingTestDay()
        {
            var random = new Random();
            var daysToAdd = random.Next(1, 30);
            var testDay = DateTime.UtcNow.AddDays(daysToAdd);
            return testDay;
        }

    }

}