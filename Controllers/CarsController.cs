using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TrafficAPI.Models;

namespace TrafficAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly IMongoCollection<Car> _cars;

        public CarsController(IMongoClient client)
        {
            var database = client.GetDatabase("traffic");
            _cars = database.GetCollection<Car>("cars");
        }

        [HttpGet("{plateNumber}")]
        public async Task<ActionResult<Car>> GetCar(string plateNumber)
        {
            // query the MongoDB collection for the car with the given plate number
            var car = await _cars.Find(c => c.PlateNumber == plateNumber).FirstOrDefaultAsync();
            if (car == null)
            {
                return NotFound();
            }
            return car;
        }

        [HttpGet("GenerateAndInsertCars")]
        public IActionResult GenerateAndInsertCars()
        {
            // Call the GenerateCars method to generate 100 instances of the Car class
            var cars = GenerateAndInsertCars(100, this);

            // Return the generated cars
            return Ok(cars);
        }

        static string[] GenerateArabicFirstNames()
        {
            // create a list of Arabic first names
            var arabicFirstNames = new string[]
            {
                "أحمد", "محمد", "حسن", "علي", "عبدلله", "عبدالرحمن", "سعد", "عمر", "عماد", "مصطفى", "عزام", "فهد", "ماجد", "ياسر", "عبدلطيف", "جمال", "أسامة", "عبدالمجيد", "عبدالحميد", "صالح", "فارس", "خالد", "علاء", "أيمن", "أسعد", "مروان", "مالك", "طارق", "عبدالفتاح", "محمود", "صلاح", "ناصر", "فيصل", "هاني", "عبدالقادر", "فراس", "حمد", "أمجد", "رامي", "مراد", "نواف", "علياء", "سامي", "رضا", "عادل", "محمدحسن", "عبدالله", "عبدالعزيز", "عبدالوهاب", "عبدالله", "خالد", "سلمان", "محمد", "عائض", "نبيل", "عبدالمحسن", "عبدالعزيز"
            };

            return arabicFirstNames;
        }

        static string GeneratePlateNumber()
        {
            var arabicLetters = new string[] { "أ", "ب", "ت", "ث", "ج", "ح", "خ", "د", "ذ", "ر", "ز", "س", "ش", "ص", "ض", "ط", "ظ", "ع", "غ", "ف", "ق", "ك", "ل", "م", "ن", "ه", "و", "ي" };
            var random = new Random();
            var plateNumber = "";

            // generate 2 or 3 random Arabic letters
            var numLetters = random.Next(2, 4);
            for (var i = 0; i < numLetters; i++)
            {
                plateNumber += arabicLetters[random.Next(0, arabicLetters.Length)] + " ";
            }

            // generate a random 4-digit number
            var number = random.Next(1, 10000);

            // format the plate number string
            plateNumber += number.ToString("D4");

            return plateNumber;
        }

        static string[] GenerateCarModels()
        {
            // create a list of car models
            var carModels = new string[]
            {
                "Toyota Corolla", "Toyota Camry", "Honda Civic", "Honda Accord", "Nissan Altima", "Hyundai Elantra", "Kia Optima", "Mazda 3", "Chevrolet Malibu", "Ford Fusion", "Volkswagen Jetta", "BMW 3 Series", "Mercedes-Benz C-Class", "Audi A4", "Lexus ES", "Infiniti Q50", "Cadillac ATS", "Volvo S60", "Jaguar XE", "Tesla Model 3"
            };

            return carModels;
        }

        static string[] GenerateCarColors()
        {
            // create a list of car colors
            var carColors = new string[]
            {
                "Black", "White", "Gray", "Silver", "Red", "Blue", "Green", "Yellow", "Orange", "Brown"
            };

            return carColors;
        }

        static List<Car> GenerateAndInsertCars(int count, CarsController controller)
        {
            var carModels  = GenerateCarModels();
            var carColors  = GenerateCarColors();
            var ownerNames = GenerateArabicFirstNames();
            var cars       = new List<Car>();

            for (int i = 0; i < count; i++)
            {
                var car = new Car { PlateNumber = GeneratePlateNumber(),
                    Model                       = carModels[new Random().Next(carModels.Length)],
                    Color                       = carColors[new Random().Next(carColors.Length)],
                    Year                        = new Random().Next(2000, 2022),
                    Cylinders                   = new Random().Next(4,    12),
                    MotorCapacity               = new Random().Next(1000, 7000),
                    CheckingDate                = DateTime.Today.AddDays(new Random().Next(-365,  0)),
                    LicenseCreationDate         = DateTime.Today.AddDays(new Random().Next(-3650, -365))
                };

                car.LicenseExpirationDate = car.LicenseCreationDate.AddYears(3);
                // generate an Arabic name for the car owner
                var ownerName = "";
                for (var j = 0; j < 5; j++)
                {
                    ownerName += ownerNames[new Random().Next(0, ownerNames.Length)] + " ";
                }
                car.OwnerName = ownerName.Trim();
                cars.Add(car);
                controller._cars.InsertOne(car);
            }

            return cars;
        }

    }
}