using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TrafficAPI.Models;

namespace TrafficAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly IMongoCollection<Ticket> _tickets;

        public TicketsController(IMongoClient client)
        {
            var database = client.GetDatabase("traffic");
            _tickets = database.GetCollection<Ticket>("tickets");
        }

        [HttpPost("AddTicket")]
        public async Task<ActionResult> AddTicket(Ticket ticket)
        {
            // add the ticket to the MongoDB collection
            await _tickets.InsertOneAsync(ticket);
            return Ok($"A ticket was created successfully. Ticket type: {ticket.Type}, price: {ticket.Price}");
        }

        [HttpGet("GetTickets")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets(string carPlateNumber)
        {
            var filter = Builders<Ticket>.Filter.Eq(t => t.CarPlateNumber, carPlateNumber);
            var tickets = await _tickets.Find(filter).ToListAsync();
            return Ok(new { tickets });
        }

    }
}