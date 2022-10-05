﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContextSample.Entities.Entities
{
    [Table("Person")]
    public partial class Person : EntityBase
    {
        public Person()
            => EntityConstructorPartial();

        partial void EntityConstructorPartial();

        [Key]
        public int Id { get; set; }
        [StringLength(200)]
        public string FirstName { get; set; }
        [StringLength(200)]
        public string LastName { get; set; }
    }
}
