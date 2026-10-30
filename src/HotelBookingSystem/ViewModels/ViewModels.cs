using HotelBookingSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.ViewModels
{
    public class HomeViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        [Range(1, 10, ErrorMessage = "Guests must be between 1 and 10.")]
        [Display(Name = "Guests")]
        public int Guests { get; set; } = 2;

        public IEnumerable<Room> FeaturedRooms { get; set; } = new List<Room>();
        public int TotalRoomsAvailable { get; set; }
    }

    public class RoomSearchViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime? CheckInDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime? CheckOutDate { get; set; }

        [Display(Name = "Guests")]
        [Range(1, 10, ErrorMessage = "Guests count must be at least 1.")]
        public int? Guests { get; set; }

        [Display(Name = "Room Type")]
        public string? RoomType { get; set; }

        [Display(Name = "Min Price")]
        [Range(0, 100000, ErrorMessage = "Invalid minimum price.")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Max Price")]
        [Range(0, 100000, ErrorMessage = "Invalid maximum price.")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Sort By")]
        public string? SortBy { get; set; } // "price_asc", "price_desc", "capacity_desc"

        public IEnumerable<Room> Rooms { get; set; } = new List<Room>();
        public IEnumerable<string> AvailableRoomTypes { get; set; } = new List<string>();

        public int TotalResultsCount { get; set; }
        public bool HasSearchedDates => CheckInDate.HasValue && CheckOutDate.HasValue;
    }

    public class RoomDetailsViewModel
    {
        public Room Room { get; set; } = null!;
        public IEnumerable<Room> SimilarRooms { get; set; } = new List<Room>();

        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        public int Guests { get; set; } = 2;
        public decimal EstimatedTotal { get; set; }
        public int NumberOfNights { get; set; } = 1;
        public bool IsAvailableForSelectedDates { get; set; } = true;
    }

    public class BookingCreateViewModel
    {
        [Required]
        public int RoomId { get; set; }

        // Snapshot details of the room for display
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Amenities { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string GuestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(30, ErrorMessage = "Phone number cannot exceed 30 characters.")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Check-in date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Check-out date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Number of guests is required.")]
        [Range(1, 20, ErrorMessage = "Guests count must be between 1 and 20.")]
        [Display(Name = "Number of Guests")]
        public int GuestsCount { get; set; } = 1;

        [StringLength(1000, ErrorMessage = "Special requests cannot exceed 1000 characters.")]
        [Display(Name = "Special Requests (Optional)")]
        public string? SpecialRequests { get; set; }

        // Server calculated (never trust browser-provided total)
        public int CalculatedNights { get; set; }
        public decimal CalculatedTotalAmount { get; set; }
    }

    public class BookingConfirmationViewModel
    {
        public Booking Booking { get; set; } = null!;
        public Room Room { get; set; } = null!;
        public int Nights { get; set; }
    }

    public class MyBookingLookupViewModel
    {
        [Required(ErrorMessage = "Booking ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid Booking ID.")]
        [Display(Name = "Booking ID")]
        public int? BookingId { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter the email address used when booking.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        public Booking? BookingResult { get; set; }
        public bool HasSearched { get; set; }
        public string? SearchError { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int ActiveRooms { get; set; }
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int PendingBookings { get; set; }
        public int CancelledBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public double OccupancyRate { get; set; }

        public IEnumerable<Booking> RecentBookings { get; set; } = new List<Booking>();
        public IEnumerable<RoomOccupancySummary> RoomOccupancies { get; set; } = new List<RoomOccupancySummary>();
    }

    public class RoomOccupancySummary
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public bool IsOccupiedToday { get; set; }
        public int TotalBookingsCount { get; set; }
        public string? CurrentGuestName { get; set; }
    }

    public class RoomAdminViewModel
    {
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Room number is required.")]
        [StringLength(20, ErrorMessage = "Room number cannot exceed 20 characters.")]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Room type is required.")]
        [StringLength(50, ErrorMessage = "Room type cannot exceed 50 characters.")]
        [Display(Name = "Room Type")]
        public string RoomType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price per night is required.")]
        [Range(0.01, 100000.00, ErrorMessage = "Price per night must be greater than 0.")]
        [Display(Name = "Price Per Night ($)")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20.")]
        [Display(Name = "Maximum Guest Capacity")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        [StringLength(1000, ErrorMessage = "Image URL cannot exceed 1000 characters.")]
        [Display(Name = "Room Image URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Amenities (comma-separated)")]
        [StringLength(1000, ErrorMessage = "Amenities cannot exceed 1000 characters.")]
        public string Amenities { get; set; } = string.Empty;

        [Display(Name = "Room Is Active")]
        public bool IsActive { get; set; } = true;

        public int TotalBookingsCount { get; set; }
        public int ActiveBookingsCount { get; set; }
    }

    public class BookingAdminFilterViewModel
    {
        public string? Status { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Admin Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
