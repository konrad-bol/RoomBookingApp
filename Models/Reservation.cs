using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RoombookingApp.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int UserId { get;set; }

        public DateTime ReservationFrom { get; set; }
        public DateTime ReservationTo { get; set; }

        [JsonIgnore]
        [NotNull]
        public Room? Room { get; set; }

        public bool Archived { get; set; } = false;
    }
}
