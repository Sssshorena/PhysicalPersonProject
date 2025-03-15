using Microsoft.EntityFrameworkCore;
using Shared.Models;



    namespace Project.Infrastructure
    {
        public class PhysicalPersonsDbContext : DbContext
        {
            public PhysicalPersonsDbContext(DbContextOptions<PhysicalPersonsDbContext> options) : base(options)
            {
            }
            public DbSet<PhysicalPerson> PhysicalPersons { get; set; }
            public DbSet<PhoneNumber> PhoneNumbers { get; set; }
            public DbSet<RelatedPerson> RelatedPersons { get; set; }
            public DbSet<City> Cities { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<RelatedPerson>()
                .HasOne(rp => rp.Person)
                .WithMany(p => p.RelatedPersons)
                .HasForeignKey(rp => rp.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<RelatedPerson>()
                .HasOne(rp => rp.Related)
                .WithMany()
                .HasForeignKey(rp => rp.RelatedPersonId)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<PhoneNumber>()
                .HasOne(p => p.PhysicalPerson)
                .WithMany(p => p.PhoneNumbers)
                .HasForeignKey(p => p.PhysicalPersonId)
                .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }


