using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public class Room
    {
        [Key]
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
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price per night is required.")]
        [Range(0.01, 100000.00, ErrorMessage = "Price per night must be greater than zero.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Price Per Night ($)")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Maximum capacity is required.")]
        [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20 guests.")]
        [Display(Name = "Maximum Capacity")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        [StringLength(1000, ErrorMessage = "Image URL cannot exceed 1000 characters.")]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Amenities string cannot exceed 1000 characters.")]
        [Display(Name = "Amenities")]
        public string Amenities { get; set; } = string.Empty;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created At (UTC)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
