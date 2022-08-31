using DBContextSample.Entities.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DBContextSample.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly CoreContext _context;

        public PersonController(CoreContext context)
            => _context = context;



        [HttpGet("Get")]
        public IEnumerable<Person> Get()
            => _context.People
                .AsNoTracking()
                .ToList();
    }
}
