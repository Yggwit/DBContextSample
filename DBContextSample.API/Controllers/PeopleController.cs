using DBContextSample.Entities.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DBContextSample.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly CoreContext _context;

        public PeopleController(CoreContext context)
            => _context = context;


        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPeople()
            => _context.People is not null
                ? await _context.People.ToListAsync()
                : NotFound();

        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            if (_context.People is null)
                return NotFound();

            return await _context.People.FindAsync(id) is Person person
                ? person
                : NotFound();
        }

        // PUT: api/People/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/People
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            if (_context.People is null)
                return Problem("Entity set 'CoreContext.People'  is null.");

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerson", new { id = person.Id }, person);
        }

        // DELETE: api/People/5
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
