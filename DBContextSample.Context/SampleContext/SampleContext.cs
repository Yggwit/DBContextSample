using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DBContextSample.Entities.Entities;

namespace DBContextSample.Context
{
    public abstract partial class SampleContext : DbContext
    {
        public virtual DbSet<Person> People { get; set; }

        public SampleContext(DbContextOptions<SampleContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
