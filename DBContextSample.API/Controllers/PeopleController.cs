using DBContextSample.API.Services;
using DBContextSample.API.Sieve;
using DBContextSample.Entities.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DBContextSample.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly CoreContext _context;
        private readonly FilterService _filterService;

        public PeopleController(CoreContext context, FilterService filterService)
        {
            _context = context;
            _filterService = filterService;
        }


        // GET: api/people?sorts=-firstName&page=1&pageSize=50&fields=firstName,lastName
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPeople([FromQuery] FilterModel<Person> filter)
            => _context.People is not null
                ? await _filterService.Filter<Person>(filter)
                : NotFound();


        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            if (_context.People is null)
                return NotFound();

            return await _context.People.FindAsync(id) is Person person
                ? person
                : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(int id, Person person)
        {
            if (id != person.Id)
                return BadRequest();

            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                    NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            if (_context.People is null)
                return Problem("Entity set 'CoreContext.People'  is null.");

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerson", new { id = person.Id }, person);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            if (_context.People is null)
                return NotFound();

            if (await _context.People.FindAsync(id) is Person person)
            {
                _context.People.Remove(person);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            else
                return NotFound();
        }

        private bool PersonExists(int id)
            => _context.People
                ?.Any(e => e.Id == id)
                ?? default;
    }
}
