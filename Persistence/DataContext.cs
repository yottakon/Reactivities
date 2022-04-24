using Microsoft.EntityFrameworkCore;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Persistence
{
    //Persistence creates a session from a database for querying and save instances
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        //Creates a Database set of Activities, created in Domain
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityAttendee> ActivityAttendees { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Comment> Comments {get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        
            //Get ActivityAttendee entity
            builder.Entity<ActivityAttendee>(x => x.HasKey(aa => new {aa.AppUserId, aa.ActivityId}));

            //Creates many to many relationship with activity and appusers
            builder.Entity<ActivityAttendee>()
            .HasOne(u => u.AppUser)
            .WithMany(a => a.Activities)
            .HasForeignKey(aa => aa.AppUserId);

            builder.Entity<ActivityAttendee>()
            .HasOne(u => u.Activity)
            .WithMany(a => a.Attendees)
            .HasForeignKey(aa => aa.ActivityId);

            builder.Entity<Comment>()
            .HasOne(a => a.Activity)
            .WithMany(c => c.Comments)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}