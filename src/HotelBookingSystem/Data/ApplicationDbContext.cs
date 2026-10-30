using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Room configuration
            builder.Entity<Room>(entity =>
            {
                entity.HasIndex(r => r.RoomNumber)
                      .IsUnique()
                      .HasDatabaseName("IX_Rooms_RoomNumber");

                entity.Property(r => r.PricePerNight)
                      .HasColumnType("decimal(18,2)");

                entity.Property(r => r.RoomNumber)
                      .HasMaxLength(20);

                entity.Property(r => r.RoomType)
                      .HasMaxLength(50);
            });

            // Booking configuration
            builder.Entity<Booking>(entity =>
            {
                entity.HasIndex(b => b.BookingReference)
                      .IsUnique()
                      .HasDatabaseName("IX_Bookings_BookingReference");

                entity.Property(b => b.PricePerNight)
                      .HasColumnType("decimal(18,2)");

                entity.Property(b => b.TotalAmount)
                      .HasColumnType("decimal(18,2)");

                entity.Property(b => b.BookingReference)
                      .HasMaxLength(30);

                entity.Property(b => b.BookingStatus)
                      .HasMaxLength(30);

                // Foreign key constraint with Restrict behavior to prevent deleting a room with bookings
                entity.HasOne(b => b.Room)
                      .WithMany(r => r.Bookings)
                      .HasForeignKey(b => b.RoomId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
