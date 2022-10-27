using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContextSample.Entities.Entities
{
    [Keyless]
    public partial class VwPerson : IEntityBase
    {
        public VwPerson()
            => EntityConstructorPartial();

        partial void EntityConstructorPartial();

        [Column("ID")]
        public int Id { get; set; }
        [StringLength(200)]
        public string FirstName { get; set; }
        [StringLength(200)]
        public string LastName { get; set; }
    }
}
