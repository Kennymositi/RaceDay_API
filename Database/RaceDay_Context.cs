using Microsoft.EntityFrameworkCore;
using RaceDay_API.Models;

namespace RaceDay_API.Database
{
    public class RaceDay_Context : DbContext

    {

        public RaceDay_Context(

            DbContextOptions<RaceDay_Context> options)

            : base(options)

        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Event_Type> EventTypes { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(

            

            ModelBuilder modelBuilder)

        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Enrollment>()
    .HasOne(e => e.User)
    .WithMany(u => u.Enrollments)
    .HasForeignKey(e => e.participant_id);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Event)
                .WithMany()
                .HasForeignKey(e => e.event_id);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.category_id);

            modelBuilder.Entity<Event_Type>().ToTable("EVENTTYPE");

            // Each user must have a unique email address.

            modelBuilder.Entity<User>()

                .HasIndex(u => u.email)

                .IsUnique();

            // Each enrollment can have at most one result.

            modelBuilder.Entity<Result>()

                .HasIndex(r => r.enrollment_id)

                .IsUnique();

            // An event must have a positive distance.

            modelBuilder.Entity<Event>()

                .ToTable(t => t.HasCheckConstraint(

                    "CK_EVENT_Distance",

                    "[distance] > 0"));

            // A finishing position must be positive.

            modelBuilder.Entity<Result>()

                .ToTable(t => t.HasCheckConstraint(

                    "CK_RESULT_FinishingPosition",

                    "[finishing_position] > 0"));

        }

    }

}