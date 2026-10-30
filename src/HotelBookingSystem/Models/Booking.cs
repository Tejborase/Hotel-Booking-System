using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        [Display(Name = "Room")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Guest name is required.")]
        [StringLength(150, ErrorMessage = "Guest name cannot exceed 150 characters.")]
        [Display(Name = "Guest Name")]
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
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; }

        [Required(ErrorMessage = "Number of guests is required.")]
        [Range(1, 20, ErrorMessage = "Guests count must be at least 1.")]
        [Display(Name = "Number of Guests")]
        public int GuestsCount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Price Per Night ($)")]
        public decimal PricePerNight { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount ($)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Status")]
        public string BookingStatus { get; set; } = "Confirmed"; // Pending, Confirmed, Cancelled

        [StringLength(1000, ErrorMessage = "Special requests cannot exceed 1000 characters.")]
        [Display(Name = "Special Requests")]
        public string? SpecialRequests { get; set; }

        [Display(Name = "Booked On (UTC)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }
    }
}
