IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'HotelBookingDB')
BEGIN
    CREATE DATABASE [HotelBookingDB];
END;
GO

USE [HotelBookingDB];
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE TABLE [Rooms] (
        [RoomId] int NOT NULL IDENTITY,
        [RoomNumber] nvarchar(20) NOT NULL,
        [RoomType] nvarchar(50) NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [PricePerNight] decimal(18,2) NOT NULL,
        [Capacity] int NOT NULL,
        [ImageUrl] nvarchar(1000) NOT NULL,
        [Amenities] nvarchar(1000) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Rooms] PRIMARY KEY ([RoomId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE TABLE [Bookings] (
        [BookingId] int NOT NULL IDENTITY,
        [BookingReference] nvarchar(30) NOT NULL,
        [RoomId] int NOT NULL,
        [GuestName] nvarchar(150) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [Phone] nvarchar(30) NOT NULL,
        [CheckInDate] datetime2 NOT NULL,
        [CheckOutDate] datetime2 NOT NULL,
        [GuestsCount] int NOT NULL,
        [PricePerNight] decimal(18,2) NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [BookingStatus] nvarchar(30) NOT NULL,
        [SpecialRequests] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Bookings] PRIMARY KEY ([BookingId]),
        CONSTRAINT [FK_Bookings_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([RoomId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Bookings_BookingReference] ON [Bookings] ([BookingReference]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bookings_RoomId] ON [Bookings] ([RoomId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Rooms_RoomNumber] ON [Rooms] ([RoomNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030093849_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261030093849_InitialCreate', N'8.0.11');
END;
GO

COMMIT;
GO

-- --------------------------------------------------
-- SEED DATA: Identity Roles & Admin User
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [Name] = N'Admin')
BEGIN
    INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (N'7866f332-c2b8-4678-9419-f328e0865aac', N'Admin', N'ADMIN', NEWID());
END;
GO

IF NOT EXISTS (SELECT 1 FROM [AspNetUsers] WHERE [Email] = N'admin@grandvista.com')
BEGIN
    -- Password is: GrandVista@2026!
    INSERT INTO [AspNetUsers] (
        [Id], [FullName], [CreatedAt], [UserName], [NormalizedUserName], [Email], [NormalizedEmail],
        [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber],
        [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]
    )
    VALUES (
        N'e902eada-a1ef-4b8d-a628-f8c05bc7de2e', N'GrandVista Administrator', SYSUTCDATETIME(),
        N'admin@grandvista.com', N'ADMIN@GRANDVISTA.COM', N'admin@grandvista.com', N'ADMIN@GRANDVISTA.COM',
        1, N'AQAAAAIAAYagAAAAENK5jDqX3yvWqZ1h765rXGZpG8lQxQfB83Vn0FpB3Xy5J7L1G5vB2p5k9L0v7B6A==',
        N'SECURITYSTAMP1234567890', NEWID(), NULL, 0, 0, NULL, 1, 0
    );

    INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
    VALUES (N'e902eada-a1ef-4b8d-a628-f8c05bc7de2e', N'7866f332-c2b8-4678-9419-f328e0865aac');
END;
GO

-- --------------------------------------------------
-- SEED DATA: Luxury Hotel Rooms
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [Rooms])
BEGIN
    SET IDENTITY_INSERT [Rooms] ON;

    INSERT INTO [Rooms] ([RoomId], [RoomNumber], [RoomType], [Description], [PricePerNight], [Capacity], [ImageUrl], [Amenities], [IsActive], [CreatedAt])
    VALUES 
    (1, N'101', N'Executive King Suite', N'Immerse yourself in elevated luxury with our Executive King Suite. Features a handcrafted king-size pillowtop bed, panoramic city vistas, a custom marble bathroom with rain shower, and a private workstation tailored for the discerning traveler.', 320.00, 2, N'https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=1200&q=80', N'King Bed, City Skyline View, Marble Bathroom, Free High-Speed Wi-Fi, Nespresso Machine, Smart 4K TV, 24/7 Room Service, In-Room Safe', 1, SYSUTCDATETIME()),
    (2, N'201', N'Deluxe Oceanfront Suite', N'Wake up to breathtaking ocean sunrises. This expansive suite offers floor-to-ceiling glass doors opening onto a private furnished terrace, plush king bedding, an oversized spa bathtub, and dedicated evening turndown service.', 450.00, 2, N'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?auto=format&fit=crop&w=1200&q=80', N'Panoramic Ocean View, Private Balcony, King Bed, Jacuzzi Tub, Bathrobes & Slivers, High-Speed Wi-Fi, Mini Bar, Butler Service', 1, SYSUTCDATETIME()),
    (3, N'301', N'Grand Family Residence', N'Designed for family comfort without sacrificing luxury. Features two separate queen bedrooms, an inviting designer lounge area, two marble bathrooms, dining table, and premium entertainment systems for all ages.', 520.00, 5, N'https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=1200&q=80', N'2 Queen Beds + Sofa, 2 Full Bathrooms, Lounge Area, Dining Table, Child-Friendly Amenities, Smart TV, Free Wi-Fi, Kitchenette', 1, SYSUTCDATETIME()),
    (4, N'401', N'Signature Royal Villa', N'Our signature luxury retreat boasting an exclusive private heated plunge pool, tranquil sundeck, handcrafted timber finishes, walk-in dressing room, and bespoke sommelier-curated wine selection.', 780.00, 4, N'https://images.unsplash.com/photo-1613977257363-707ba9348227?auto=format&fit=crop&w=1200&q=80', N'Private Heated Plunge Pool, Sun Terrace, 2 Bedrooms, Walk-in Closet, Wine Cellar, Dedicated Butler, Luxury Toiletries, Airport Chauffeur', 1, SYSUTCDATETIME()),
    (5, N'501', N'Presidential Penthouse', N'The pinnacle of GrandVista elegance. Occupying the hotel''s top floor, this crown jewel features 360-degree skyline views, private elevator access, baby grand piano, infinity hydrotherapy spa, and private chef capabilities.', 980.00, 4, N'https://images.unsplash.com/photo-1591088398332-8a7791972843?auto=format&fit=crop&w=1200&q=80', N'Top Floor Penthouse, 360-Degree Views, Private Elevator, Hydrotherapy Spa, Chef Kitchen, Executive Lounge Access, Premium Bar, Chauffeur', 1, SYSUTCDATETIME()),
    (6, N'102', N'Junior Courtyard Suite', N'A peaceful sanctuary overlooking our landscaped botanical courtyard. Features a plush queen bed, rainfall walk-in shower, acoustic soundproofing, and an artisan tea and coffee bar.', 260.00, 2, N'https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=1200&q=80', N'Queen Bed, Courtyard Garden View, Rainfall Shower, High-Speed Wi-Fi, Artisan Tea Bar, Smart TV, Workspace', 1, SYSUTCDATETIME()),
    (7, N'202', N'Superior Double Room', N'A refined modern guestroom crafted for seamless relaxation. Offering twin premium beds or a plush queen, bespoke oak vanity, and intuitive bedside environmental controls.', 220.00, 2, N'https://images.unsplash.com/photo-1595576508898-0ad5c879a061?auto=format&fit=crop&w=1200&q=80', N'Queen Bed, Walk-in Rain Shower, High-Speed Wi-Fi, Bedside Lighting Controls, Safe, USB Ports, Smart TV', 1, SYSUTCDATETIME()),
    (8, N'302', N'Boutique Heritage Suite', N'Blending historic architectural elegance with contemporary luxury. Features exposed antique woodwork, an ornamental fireplace, deep freestanding soaking tub, and bespoke velvet upholstery.', 380.00, 3, N'https://images.unsplash.com/photo-1578683010236-d716f9a3f461?auto=format&fit=crop&w=1200&q=80', N'King Bed + Daybed, Freestanding Soaking Tub, Heritage Fireplace, Designer Bathrobes, Wi-Fi, Marshall Speaker, Mini Bar', 1, SYSUTCDATETIME());

    SET IDENTITY_INSERT [Rooms] OFF;
END;
GO


