using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RoombookingApp.Models
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [JsonIgnore]
        public List<Reservation> Reservations { get; set; } = new();
    }
    public class RoomAvailabilityRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
    public class RoomRequest
    {
        public string? Name { get; set; }
    }
    public class ReservationRequest
    {
        public RoomAvailabilityRequest TimeRequest { get; set; }
        public Room Room { get; set; }
    }

}
