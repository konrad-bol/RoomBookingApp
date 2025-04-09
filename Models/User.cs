using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RoombookingApp.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [NotNull]
        public string? Name { get; set; }
        [Required]
        [NotNull]
        public string? Email { get; set; }
        [Required]
        [NotNull]
        public string? PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}
