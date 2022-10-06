using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContextSample.Entities.Entities
{
    [Table("Person")]
    public partial class Person : IEntityBase
    {
        public Person()
            => EntityConstructorPartial();

        partial void EntityConstructorPartial();

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        public Guid Guid { get; set; }
        [StringLength(200)]
        public string FirstName { get; set; }
        [StringLength(200)]
        public string LastName { get; set; }
    }
}
