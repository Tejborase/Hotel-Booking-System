using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. Ensure Database & Migrations
            await context.Database.MigrateAsync();

            // 2. Seed Roles
            string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // 3. Seed Admin User
            string adminEmail = configuration["AdminSettings:Email"] ?? "admin@grandvista.com";
            string adminPassword = configuration["AdminSettings:Password"] ?? "GrandVista@2026!";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "GrandVista Administrator",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
            }

            // 4. Seed Luxury Hotel Rooms
            if (!await context.Rooms.AnyAsync())
            {
                var rooms = new List<Room>
                {
                    new Room
                    {
                        RoomNumber = "101",
                        RoomType = "Executive King Suite",
                        Description = "Immerse yourself in elevated luxury with our Executive King Suite. Features a handcrafted king-size pillowtop bed, panoramic city vistas, a custom marble bathroom with rain shower, and a private workstation tailored for the discerning traveler.",
                        PricePerNight = 320.00m,
                        Capacity = 2,
                        ImageUrl = "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=1200&q=80",
                        Amenities = "King Bed, City Skyline View, Marble Bathroom, Free High-Speed Wi-Fi, Nespresso Machine, Smart 4K TV, 24/7 Room Service, In-Room Safe",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-3)
                    },
                    new Room
                    {
                        RoomNumber = "201",
                        RoomType = "Deluxe Oceanfront Suite",
                        Description = "Wake up to breathtaking ocean sunrises. This expansive suite offers floor-to-ceiling glass doors opening onto a private furnished terrace, plush king bedding, an oversized spa bathtub, and dedicated evening turndown service.",
                        PricePerNight = 450.00m,
                        Capacity = 2,
                        ImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?auto=format&fit=crop&w=1200&q=80",
                        Amenities = "Panoramic Ocean View, Private Balcony, King Bed, Jacuzzi Tub, Bathrobes & Slivers, High-Speed Wi-Fi, Mini Bar, Butler Service",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-3)
                    },
                    new Room
                    {
                        RoomNumber = "301",
                        RoomType = "Grand Family Residence",
                        Description = "Designed for family comfort without sacrificing luxury. Features two separate queen bedrooms, an inviting designer lounge area, two marble bathrooms, dining table, and premium entertainment systems for all ages.",
                        PricePerNight = 520.00m,
                        Capacity = 5,
                        ImageUrl = "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=1200&q=80",
                        Amenities = "2 Queen Beds + Sofa, 2 Full Bathrooms, Lounge Area, Dining Table, Child-Friendly Amenities, Smart TV, Free Wi-Fi, Kitchenette",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-2)
                    },
                    new Room
                    {
                        RoomNumber = "401",
                        RoomType = "Signature Royal Villa",
                        Description = "Our signature luxury retreat boasting an exclusive private heated plunge pool, tranquil sundeck, handcrafted timber finishes, walk-in dressing room, and bespoke sommelier-curated wine selection.",
                        PricePerNight = 780.00m,
                        Capacity = 4,
                        ImageUrl = "https://images.unsplash.com/photo-1613977257363-707ba9348227?auto=format&fit=crop&w=1200&q=80",
                        Amenities = "Private Heated Plunge Pool, Sun Terrace, 2 Bedrooms, Walk-in Closet, Wine Cellar, Dedicated Butler, Luxury Toiletries, Airport Chauffeur",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-2)
                    },
                    new Room
                    {
                        RoomNumber = "501",
                        RoomType = "Presidential Penthouse",
                        Description = "The pinnacle of GrandVista elegance. Occupying the hotel's top floor, this crown jewel features 360-degree skyline views, private elevator access, baby grand piano, infinity hydrotherapy spa, and private chef capabilities.",
                        PricePerNight = 980.00m,
                        Capacity = 4,
                        ImageUrl = "https://images.unsplash.com/photo-1591088398332-8a7791972843?auto=format&fit=crop&w=1200&q=80",
                        Amenities = "Top Floor Penthouse, 360-Degree Views, Private Elevator, Hydrotherapy Spa, Chef Kitchen, Executive Lounge Access, Premium Bar, Chauffeur",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-1)
                    },
                    new Room
                    {
                        RoomNumber = "102",
                        RoomType = "Junior Courtyard Suite",
                        Description = "A peaceful sanctuary overlooking our landscaped botanical courtyard. Features a plush queen bed, rainfall walk-in shower, acoustic soundproofing, and an artisan tea and coffee bar.",
                        PricePerNight = 260.00m,
                        Capacity = 2,
                        ImageUrl = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=1200&q=80",
                        Amenities = "Queen Bed, Courtyard Garden View, Rainfall Shower, High-Speed Wi-Fi, Artisan Tea Bar, Smart TV, Workspace",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-1)
                    },
                    new Room
                    {
                        RoomNumber = "202",
                        RoomType = "Superior Double Room",
                        Description = "A refined modern guestroom crafted for seamless relaxation. Offering twin premium beds or a plush queen, bespoke oak vanity, and intuitive bedside environmental controls.",
                        PricePerNight = 220.00m,
                        Capacity = 2,
                        ImageUrl = "https://images.unsplash.com/photo-1595576508898-0ad5c879a061?auto=format&fit=crop&w=1200&q=80",
                        Amenities = "Queen Bed, Walk-in Rain Shower, High-Speed Wi-Fi, Bedside Lighting Controls, Safe, USB Ports, Smart TV",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-1)
                    },
                    new Room
                    {
                        RoomNumber = "302",
                        RoomType = "Boutique Heritage Suite",
                        Description = "Blending historic architectural elegance with contemporary luxury. Features exposed antique woodwork, an ornamental fireplace, deep freestanding soaking tub, and bespoke velvet upholstery.",
                        PricePerNight = 380.00m,
                        Capacity = 3,
                        ImageUrl = "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?auto=format&fit=crop&w=1200&q=80",
                        Amenities = "King Bed + Daybed, Freestanding Soaking Tub, Heritage Fireplace, Designer Bathrobes, Wi-Fi, Marshall Speaker, Mini Bar",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-1)
                    }
                };

                await context.Rooms.AddRangeAsync(rooms);
                await context.SaveChangesAsync();

                // 5. Seed Initial Realistic Sample Bookings for Dashboard and Management
                var room101 = rooms[0];
                var room201 = rooms[1];
                var room301 = rooms[2];

                var today = DateTime.Today;

                var sampleBookings = new List<Booking>
                {
                    new Booking
                    {
                        RoomId = room101.RoomId,
                        GuestName = "Eleanor Vance",
                        Email = "eleanor.vance@example.com",
                        Phone = "+1 (555) 234-5678",
                        CheckInDate = today.AddDays(-5),
                        CheckOutDate = today.AddDays(-2),
                        GuestsCount = 2,
                        PricePerNight = room101.PricePerNight,
                        TotalAmount = room101.PricePerNight * 3,
                        BookingStatus = "Confirmed",
                        SpecialRequests = "Late arrival around 9 PM. Anniversary celebration.",
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new Booking
                    {
                        RoomId = room201.RoomId,
                        GuestName = "Marcus Sterling",
                        Email = "marcus.sterling@example.com",
                        Phone = "+1 (555) 876-5432",
                        CheckInDate = today.AddDays(2),
                        CheckOutDate = today.AddDays(6),
                        GuestsCount = 2,
                        PricePerNight = room201.PricePerNight,
                        TotalAmount = room201.PricePerNight * 4,
                        BookingStatus = "Confirmed",
                        SpecialRequests = "High floor preferred. Airport transfer requested.",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new Booking
                    {
                        RoomId = room301.RoomId,
                        GuestName = "Sophia Laurent",
                        Email = "sophia.laurent@example.com",
                        Phone = "+1 (555) 432-1098",
                        CheckInDate = today.AddDays(7),
                        CheckOutDate = today.AddDays(10),
                        GuestsCount = 4,
                        PricePerNight = room301.PricePerNight,
                        TotalAmount = room301.PricePerNight * 3,
                        BookingStatus = "Pending",
                        SpecialRequests = "Traveling with two children, request extra towels.",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new Booking
                    {
                        RoomId = room101.RoomId,
                        GuestName = "Jonathan Davis",
                        Email = "j.davis@example.com",
                        Phone = "+1 (555) 987-1234",
                        CheckInDate = today.AddDays(-12),
                        CheckOutDate = today.AddDays(-9),
                        GuestsCount = 1,
                        PricePerNight = room101.PricePerNight,
                        TotalAmount = room101.PricePerNight * 3,
                        BookingStatus = "Cancelled",
                        SpecialRequests = "Flight cancelled.",
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    }
                };

                await context.Bookings.AddRangeAsync(sampleBookings);
                await context.SaveChangesAsync();
            }
        }
    }
}
