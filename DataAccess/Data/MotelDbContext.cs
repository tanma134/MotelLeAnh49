using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using MotelLeAnh49.Models;

namespace MotelLeAnh49.Data
{
    public class MotelDbContext : DbContext
    {
        public MotelDbContext(DbContextOptions<MotelDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ServiceItem> ServiceItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<DataAccess.Models.EventRegistration> EventRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Admin
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Username = "GiaHuy",
                    Password = "E5uUoX0wvxcHVCMSWGLQspI9EZnaUM4E4CmS0Y0epeU=",
                    FullName = "Motel Admin",
                    IsActive = true
                }
            );
        }
    }
}
