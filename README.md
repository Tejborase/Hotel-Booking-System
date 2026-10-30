# GRANDVISTA – HOTEL ROOM BOOKING SYSTEM

An enterprise-grade, luxury boutique Hotel Room Booking Web Application engineered with **ASP.NET Core MVC (.NET 8)**, **C#**, **Entity Framework Core Code First**, and **Microsoft SQL Server**.

Developed as a comprehensive technical assessment project demonstrating production-quality software architecture, strict relational database integrity, secure authentication and role-based authorization, comprehensive unit test coverage, and a bespoke luxury hospitality UI/UX design system.

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [Technology Stack](#technology-stack)
3. [Architecture & Project Structure](#architecture--project-structure)
4. [Key Features](#key-features)
   - [Customer Experience](#customer-experience)
   - [Administrative Management Console](#administrative-management-console)
5. [Database Design & Schema](#database-design--schema)
6. [Business Logic & Validation Rules](#business-logic--validation-rules)
7. [Setup & Running Instructions](#setup--running-instructions)
8. [Database Migration & Initialization](#database-migration--initialization)
9. [Administrator Access & Test Credentials](#administrator-access--test-credentials)
10. [Design System Decisions & Aesthetics](#design-system-decisions--aesthetics)
11. [Automated Testing & Verification](#automated-testing--verification)
12. [Completed Features](#completed-features)
13. [Incomplete Features & Known Issues](#incomplete-features--known-issues)

---

## 1. Project Overview

**GrandVista** is a luxury boutique hotel brand offering premium suites, personalized concierge services, and scenic coastal retreats. This platform provides an end-to-end digital hospitality experience:
- **Guests** can explore curated suites, verify real-time room availability across specific check-in and check-out dates, inspect amenities and transparent pricing, book reservations without upfront card processing (Pay at Hotel), receive printable vouchers with unique reference codes, and retrieve existing reservations securely.
- **Hotel Administrators** can monitor operational metrics (revenue, occupancy percentage, booking volume), execute complete room CRUD operations with duplicate room number guards and safe deletion checks, and manage guest reservations with strict status lifecycle enforcement.

---

## 2. Technology Stack

### Backend
- **Framework:** ASP.NET Core 8.0 MVC (C#)
- **Data Access:** Entity Framework Core 8.0 (Code First, Migrations, LINQ)
- **Security & Auth:** ASP.NET Core Identity (PBKDF2 password hashing, cookie-based session management, role-based authorization)
- **Architecture:** Layered Architecture, Repository/Service Pattern, Dependency Injection

### Database
- **Engine:** Microsoft SQL Server 2025 / 2022 / LocalDB
- **Tools:** EF Core CLI (`dotnet ef`), SQL Server Management Studio (SSMS), `sqlcmd`
- **Deliverables:** `database/HotelBookingDB.sql`, `database/HotelBookingDB.bak`, `database/ER-Diagram.png`

### Frontend & UI/UX
- **Rendering:** Razor Views (`.cshtml`)
- **CSS Framework:** Bootstrap 5.3 + Bootstrap Icons 1.11
- **Custom Design System:** Vanilla CSS (`wwwroot/css/site.css`) tailored with luxury boutique aesthetics
- **Typography:** Google Fonts (*Playfair Display* for serif luxury headings, *Inter* for clean legible body typography)
- **Client Logic:** Vanilla JavaScript (dynamic stay calculation, date range constraints, print utilities)

### Testing
- **Test Framework:** xUnit 2.5
- **Mocking/In-Memory:** EF Core InMemory Provider (`Microsoft.EntityFrameworkCore.InMemory`)

---

## 3. Architecture & Project Structure

The project follows a clean separation of concerns:

```
HotelBookingSystem/
│
├── database/
│   ├── HotelBookingDB.sql      # Full idempotent SQL creation & seed script
│   ├── HotelBookingDB.bak      # Verified SQL Server native backup file (5.59 MB)
│   └── ER-Diagram.png          # Entity-Relationship diagram
│
├── screenshots/
│   ├── 01-home.png             # Landing page with hero & live booking search
│   ├── 02-rooms.png            # Filterable room inventory & sorting
│   ├── 03-room-details.png     # Suite details, amenities, dynamic calculator
│   ├── 04-booking.png          # Reservation booking form & summary
│   ├── 05-confirmation.png     # Printable confirmation voucher with reference
│   ├── 06-admin-dashboard.png  # Live stats, occupancy, revenue & recent bookings
│   ├── 07-room-management.png  # Room CRUD table with active toggle & delete
│   └── 08-booking-management.png # Bookings management with confirm/cancel
│
├── src/
│   ├── HotelBookingSystem.sln  # Visual Studio Solution
│   │
│   ├── HotelBookingSystem/     # ASP.NET Core MVC Web Application (.NET 8)
│   │   ├── Controllers/
│   │   │   ├── HomeController.cs
│   │   │   ├── RoomsController.cs
│   │   │   ├── BookingsController.cs
│   │   │   ├── AccountController.cs
│   │   │   └── Admin/
│   │   │       ├── DashboardController.cs
│   │   │       ├── RoomsController.cs
│   │   │       └── BookingsController.cs
│   │   │
│   │   ├── Models/
│   │   │   ├── ApplicationUser.cs
│   │   │   ├── Room.cs
│   │   │   └── Booking.cs
│   │   │
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── DbInitializer.cs
│   │   │
│   │   ├── Services/
│   │   │   ├── IRoomService.cs & RoomService.cs
│   │   │   └── IBookingService.cs & BookingService.cs
│   │   │
│   │   ├── ViewModels/
│   │   │   └── ViewModels.cs
│   │   │
│   │   ├── Views/
│   │   │   ├── Home/
│   │   │   ├── Rooms/
│   │   │   ├── Bookings/
│   │   │   ├── Account/
│   │   │   ├── Admin/
│   │   │   └── Shared/
│   │   │
│   │   ├── wwwroot/
│   │   │   ├── css/site.css
│   │   │   └── js/site.js
│   │   │
│   │   ├── appsettings.json
│   │   └── Program.cs
│   │
│   └── HotelBookingSystem.Tests/ # xUnit Automated Test Suite (.NET 8)
│       ├── BookingBusinessRuleTests.cs
│       └── RoomManagementTests.cs
│
├── README.md
└── AI_TOOLS_USED.md
```

---

## 4. Key Features

### Customer Experience
1. **Home Page:**
   - Full-width hero banner with luxury photographic backdrop, dark contrast overlay, gold award badge, and CTA.
   - Interactive date & guest search widget that routes directly to filtered room listings.
   - Featured suites showcase, resort facilities (saltwater infinity pool, Michelin dining, holistic spa, private chauffeur), brand philosophy, and verified guest testimonials.
2. **Rooms Listing:**
   - Comprehensive inventory of active rooms with high-definition imagery, category pills, nightly rates, capacity badges, and amenity tags.
   - Dynamic filters: Suite category, minimum price, maximum price, guest capacity.
   - Real-time sorting: Price (Low to High), Price (High to Low), Capacity (High to Low).
   - Friendly empty state for searches yielding no matching suites.
3. **Room Details:**
   - High-resolution gallery presentation and suite highlights.
   - Amenity checklist (king bedding, ocean vistas, marble baths, espresso machines, complimentary fiber Wi-Fi).
   - Sticky reservation calculation panel calculating stay duration and total cost on the fly.
4. **Booking Form:**
   - Clean customer contact capture (Full Name, Email, Phone, Stay Dates, Guest Count, Special Requests).
   - Server-side price recalculation (browser-submitted totals are disregarded).
   - Concurrency-safe reservation creation backed by database transaction.
   - Payment recorded transparently as **"Pay at Hotel Check-in"**.
5. **Booking Confirmation:**
   - Displays uniquely generated reference code (format: `GV-YYYY-XXXXX`).
   - Complete itemized breakdown of stay schedule, guest details, room snapshot, and total amount.
   - Dedicated **Print Confirmation** button with optimized print stylesheet.
6. **My Booking (Reservation Lookup):**
   - Secure guest self-service lookup requiring **both** Booking Reference and Email Address.
   - Isolates guest data and prevents exposing reservations belonging to other customers.

### Administrative Management Console
1. **Dedicated Admin Layout:**
   - Separate forest green administrative sidebar with portal status pill, navigation links, and administrator profile badge.
2. **Executive Dashboard:**
   - Live metrics sourced directly from SQL Server:
     - Total Rooms & Active Inventory
     - Total Reservations
     - Confirmed, Pending, and Cancelled counts
     - Confirmed Revenue ($)
     - Today's Live Occupancy Rate (%)
   - Today's Room Status summary table (Vacant vs. Occupied, current guest).
   - Recent Reservations audit table.
3. **Room Management (CRUD):**
   - View, Create, and Edit luxury room specifications.
   - Uniqueness check preventing duplicate room numbers.
   - Instant room activation/deactivation toggle.
   - **Safe Deletion Protection:** Rooms with active or upcoming reservations cannot be deleted (system advises deactivation).
4. **Booking Management:**
   - Comprehensive table of all reservations with status badges (*Confirmed*, *Pending*, *Cancelled*).
   - Instant search by reference, guest name, email, phone, or room number.
   - Filter by status (*All*, *Confirmed*, *Pending*, *Cancelled*).
   - Status actions with transition guards (e.g., cancelled reservations cannot be re-confirmed if another guest has booked overlapping dates).

---

## 5. Database Design & Schema

Implemented using **Entity Framework Core Code First** with SQL Server.

### Rooms Table (`Rooms`)
| Column | Type | Constraints | Description |
|---|---|---|---|
| `RoomId` | `int` | Primary Key, Identity | Unique room identifier |
| `RoomNumber` | `nvarchar(20)` | Required, Unique Index (`IX_Rooms_RoomNumber`) | Room number (e.g. 101, 201) |
| `RoomType` | `nvarchar(50)` | Required | Category (e.g. Executive King Suite) |
| `Description` | `nvarchar(2000)`| Required | Detailed marketing overview |
| `PricePerNight`| `decimal(18,2)` | Required, Check > 0 | Nightly agreed tariff |
| `Capacity` | `int` | Required, Check 1-20 | Maximum allowed guests |
| `ImageUrl` | `nvarchar(1000)`| Required | High-resolution image link |
| `Amenities` | `nvarchar(1000)`| Optional | Comma-separated amenity list |
| `IsActive` | `bit` | Default `1` | Visibility / bookable flag |
| `CreatedAt` | `datetime2` | UTC | Audit creation timestamp |

### Bookings Table (`Bookings`)
| Column | Type | Constraints | Description |
|---|---|---|---|
| `BookingId` | `int` | Primary Key, Identity | Unique booking identifier |
| `BookingReference` | `nvarchar(30)` | Required, Unique Index (`IX_Bookings_BookingReference`) | Generated reference code (`GV-2026-XXXXX`) |
| `RoomId` | `int` | Foreign Key (`Rooms.RoomId`), `ON DELETE RESTRICT` | Associated hotel room |
| `GuestName` | `nvarchar(150)` | Required | Customer full name |
| `Email` | `nvarchar(150)` | Required, Email format | Customer email |
| `Phone` | `nvarchar(30)` | Required | Contact telephone |
| `CheckInDate` | `datetime2` | Required | Check-in calendar date |
| `CheckOutDate` | `datetime2` | Required | Check-out calendar date |
| `GuestsCount` | `int` | Required, 1 to Room.Capacity | Guests staying |
| `PricePerNight` | `decimal(18,2)` | Required | Snapshot price per night at time of booking |
| `TotalAmount` | `decimal(18,2)` | Required | Server-calculated (`PricePerNight * Nights`) |
| `BookingStatus` | `nvarchar(30)` | Required (`Pending`, `Confirmed`, `Cancelled`) | Reservation status |
| `SpecialRequests` | `nvarchar(1000)`| Optional | Guest special requests |
| `CreatedAt` | `datetime2` | UTC | Audit booking timestamp |

### ASP.NET Core Identity Tables
- `AspNetUsers`: Custom user model `ApplicationUser` (`FullName`, `CreatedAt`, credentials, security stamps).
- `AspNetRoles`: System roles (`Admin`).
- `AspNetUserRoles`: User-role assignments.
- `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserTokens`, `AspNetRoleClaims`.

---

## 6. Business Logic & Validation Rules

All business rules reside in dedicated service classes (`BookingService.cs` and `RoomService.cs`):

1. **Room Availability Overlap Formula:**
   A room is unavailable if an existing booking overlaps the requested stay:
   $$\text{ExistingCheckIn} < \text{RequestedCheckOut} \quad\text{AND}\quad \text{ExistingCheckOut} > \text{RequestedCheckIn}$$
   - *Cancelled* bookings are explicitly excluded.
   - *Inactive* rooms are blocked from booking.
   - **Same-Day Turnover Allowed:** If guest A departs on Day 10 at 11:00 AM, guest B can check in on Day 10 at 3:00 PM without conflict because $\text{ExistingCheckOut} (10) > \text{RequestedCheckIn} (10)$ is `False`.
2. **Server-Side Financial Security:**
   $$\text{TotalAmount} = \text{PricePerNight} \times \text{NumberOfNights}$$
   - Browser client total amounts are never trusted or bound to the entity.
   - The agreed price per night is captured as a historical snapshot in `Bookings.PricePerNight` so subsequent room rate updates do not alter existing contracts.
3. **Input & Temporal Validation:**
   - Check-in cannot be in the past ($\text{CheckIn} \ge \text{Today}$).
   - Check-out must strictly succeed check-in ($\text{CheckOut} > \text{CheckIn}$).
   - Guest count must be between 1 and the room's maximum capacity.
4. **Concurrency Guard:**
   - Availability is re-checked inside a database transaction immediately prior to committing the reservation.
5. **Safe Room Deletion:**
   - Rooms with active or upcoming bookings cannot be deleted. Deactivation is suggested to maintain historical audit integrity.

---

## 7. Setup & Running Instructions

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Microsoft SQL Server (LocalDB, SQL Express, or Standard Developer Edition)
- Visual Studio 2022 or VS Code

### 1. Clone & Navigate
```bash
git clone <repository-url>
cd "HotelBookingSystem"
```

### 2. Configure Database Connection String
Open `src/HotelBookingSystem/appsettings.json` and adjust the connection string to match your SQL Server instance:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HotelBookingDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
}
```
> **Tip:** If using SQL Server Express, set `Server=.\\SQLEXPRESS;`. If using LocalDB, set `Server=(localdb)\\mssqllocaldb;`.

### 3. Restore & Build
```bash
dotnet restore src/HotelBookingSystem.sln
dotnet build src/HotelBookingSystem.sln
```

### 4. Run the Application
```bash
dotnet run --project src/HotelBookingSystem
```
Open your browser to:
- **Guest Website:** `http://localhost:5154`
- **Admin Portal:** `http://localhost:5154/Account/Login`

---

## 8. Database Migration & Initialization

The project is configured to **automatically migrate and seed** the SQL Server database on first startup via `DbInitializer.cs`.

### Manual Migration Commands (Optional)
If you prefer applying migrations via the CLI:
```bash
dotnet ef database update --project src/HotelBookingSystem --startup-project src/HotelBookingSystem
```

### Direct SQL Script Deployment (No .NET required)
To set up the database entirely in SQL Server Management Studio (SSMS):
1. Open `database/HotelBookingDB.sql`.
2. Execute the entire script. It creates `HotelBookingDB`, tables, indexes, admin credentials, and luxury suites automatically.

### Restoring Native SQL Server Backup
To restore the provided `.bak` archive:
```sql
RESTORE DATABASE HotelBookingDB 
FROM DISK = 'D:\Hotel  Booking System\database\HotelBookingDB.bak'
WITH REPLACE;
```

---

## 9. Administrator Access & Test Credentials

The database initializer automatically provisions an initial Administrator account:

| Role | Email | Password | Access Level |
|---|---|---|---|
| **Administrator** | `admin@grandvista.com` | `GrandVista@2026!` | Full Admin Portal (`/Admin/Dashboard`) |

---

## 10. Design System Decisions & Aesthetics

GrandVista was designed from the ground up to reflect a bespoke 5-star boutique resort:
- **Color Palette:**
  - `Forest Green (#173D32)`: Represents stability, tranquility, and natural heritage.
  - `Warm Ivory (#F5F1E8)`: Soft, inviting contrast for containers and cards.
  - `Muted Gold (#C4A46C)`: Luxury accents, badges, and primary action buttons.
  - `Off-White Canvas (#FAF9F6)`: Clean, uncluttered background with zero eye strain.
- **Typography:**
  - Headings: *Playfair Display* (classic high-fashion editorial serif).
  - Body: *Inter* (geometric, highly readable sans-serif).
- **Responsive Architecture:** Fully tested across desktop (1440px), tablet (768px), and mobile (375px/390px) viewports with collapsible navigation and scrollable responsive tables.

---

## 11. Automated Testing & Verification

A dedicated test suite in `src/HotelBookingSystem.Tests/` validates all business rules:

```bash
dotnet test src/HotelBookingSystem.Tests/HotelBookingSystem.Tests.csproj
```

### Test Coverage Results
- **Total Tests:** 12
- **Passed:** 12
- **Failed:** 0
- **Duration:** 834 ms

**Key Test Scenarios:**
1. `IsRoomAvailable_WhenNoBookingsExist_ReturnsTrue`
2. `IsRoomAvailable_WhenActiveOverlappingBookingExists_ReturnsFalse`
3. `IsRoomAvailable_WhenRequestedCheckInIsSameAsExistingCheckOut_ReturnsTrue` (Same-day turnover)
4. `IsRoomAvailable_WhenExistingBookingIsCancelled_ReturnsTrue`
5. `IsRoomAvailable_WhenRoomIsInactive_ReturnsFalse`
6. `CalculateBookingTotal_CalculatesAccuratelyBasedOnServerRate`
7. `UpdateBookingStatus_CancelledToConfirmed_FailsIfRoomNowOccupied`
8. `CreateRoom_WhenRoomNumberAlreadyExists_ReturnsFailure`
9. `CreateRoom_WithNegativePriceOrZeroCapacity_ReturnsFailure`
10. `DeleteRoom_WhenRoomHasActiveBookings_ReturnsFailureAndPreventsDelete`
11. `ToggleRoomActiveStatus_TogglesStateCorrectly`
12. `GetAllRooms_WithPriceAndCapacityFilters_ReturnsMatchingRooms`

---

## 12. Completed Features

- [x] Full ASP.NET Core 8.0 MVC architecture with Dependency Injection
- [x] Entity Framework Core Code First with migrations
- [x] SQL Server persistence with decimal precision, unique indexes, and foreign keys
- [x] ASP.NET Core Identity with Admin role and password hashing
- [x] Luxury boutique design system (Forest Green, Warm Ivory, Muted Gold)
- [x] Home Page with Hero CTA, interactive search widget, facilities, testimonials
- [x] Rooms Listing with filtering (type, price, guests) and sorting (price, capacity)
- [x] Room Details with photo gallery, amenities list, and live pricing calculator
- [x] Booking Page with client + server validation and "Pay at Hotel" terms
- [x] Booking Confirmation with unique reference number and printable view
- [x] My Booking reservation lookup with reference + email verification
- [x] Completely separate Admin Panel layout with dark sidebar
- [x] Admin Dashboard with live SQL Server statistics and occupancy rate
- [x] Room Management CRUD with duplicate room number guard and safe deletion check
- [x] Booking Management with status badges and confirm/cancel actions
- [x] 12 Passing automated unit tests for business rules
- [x] SQL Server database backup (`HotelBookingDB.bak`) and SQL script (`HotelBookingDB.sql`)
- [x] ER diagram (`database/ER-Diagram.png`) and 8 UI screenshots (`screenshots/`)

---

## 13. Incomplete Features & Known Issues

- **Real Payment Gateway Integration:** In accordance with the project specification (Section 5, Page 4), credit card payment gateways (e.g. Stripe, PayPal) are intentionally not implemented. Reservations are recorded as **Confirmed** with payment collected at hotel check-in.
- **Email Dispatching Service:** Confirmation details are presented directly on the confirmation voucher and recorded in the database. SMTP email delivery (SendGrid/MailKit) can be integrated in future phases.
