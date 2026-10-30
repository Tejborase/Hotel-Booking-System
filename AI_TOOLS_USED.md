# AI Tools Used & Development Documentation

In compliance with the project assessment guidelines (Section 14), this document provides a transparent and honest account of all AI tools, models, and generative assistants utilized during the architecture, design, and implementation of the **GrandVista Hotel Room Booking System**.

---

## 1. Primary AI Tools & Assistants

| AI Tool / Assistant | Environment / Interface | Purpose & Scope of Use |
|---|---|---|
| **Antigravity AI Assistant (Gemini 3.8 Flash)** | Antigravity IDE / Autonomous Agent Pair Programmer | - Full-stack software architecture design and scaffolding<br>- ASP.NET Core MVC controllers, models, and service classes implementation<br>- Entity Framework Core Code First configurations and migrations<br>- Writing xUnit automated unit tests for business rules<br>- Writing Razor Views and the custom GrandVista CSS design system<br>- Authoring project documentation (`README.md`, `AI_TOOLS_USED.md`) |
| **Antigravity Image Generation Tool** | DeepMind Generative Image Pipeline | - Generating the Entity Relationship Diagram (`database/ER-Diagram.png`) based on the database schema |

---

## 2. Detail of AI Assistance by Technical Domain

### 1. Architecture & Backend Implementation
- **Role:** The AI assistant was tasked with structuring the application according to ASP.NET Core MVC best practices, adhering to the clean separation of concerns:
  - Created `Services/IRoomService.cs` and `Services/IBookingService.cs` to encapsulate all availability and pricing formulas.
  - Implemented the strict date overlap formula (`ExistingCheckIn < RequestedCheckOut && ExistingCheckOut > RequestedCheckIn`).
  - Added anti-forgery tokens, server-side data annotation validations, and transaction isolation.

### 2. Database Design & EF Core
- **Role:** Guided the creation of `ApplicationDbContext.cs`, defining decimal precision `decimal(18,2)` on monetary fields, unique indexes on `RoomNumber` and `BookingReference`, and `DeleteBehavior.Restrict` on foreign key relationships.
- **Verification:** The database migrations were executed and verified against a live Microsoft SQL Server instance (`HotelBookingDB`). A verified database backup (`HotelBookingDB.bak`) was generated directly from SQL Server using T-SQL `BACKUP DATABASE`.

### 3. UI/UX Design System
- **Role:** Assisted in generating the custom CSS design system (`wwwroot/css/site.css`) using the specified luxury brand color palette:
  - Primary Forest Green: `#173D32`
  - Warm Ivory: `#F5F1E8`
  - Muted Gold: `#C4A46C`
  - Canvas: `#FAF9F6`
- Configured typography utilizing Google Fonts (*Playfair Display* and *Inter*), responsive room cards, sticky price breakdown widgets, status badges, and print styles for vouchers.

### 4. Automated Testing
- **Role:** Generated 12 unit tests using xUnit and EF Core InMemory provider covering availability calculation, turnover on same-day checkout, cancelled booking availability release, price calculations, and duplicate room number rejection.
- **Verification:** Executed via `dotnet test` with 100% pass rate (12 passed, 0 failed).

### 5. Visual Artifacts & Screenshots
- **Screenshots:** All 8 screenshots in `screenshots/` (`01-home.png` through `08-booking-management.png`) were captured directly from the live running web application using headless browser automation on Windows.
- **ER Diagram:** Generated to illustrate the relationship between `Rooms`, `Bookings`, and ASP.NET Core Identity user tables.

---

## 3. Human Review & Verification Statement

All code generated with AI assistance was compiled, tested, and validated in a real development environment with Microsoft SQL Server and .NET 8 SDK. No simulated or unverified deliverables are claimed to exist.
