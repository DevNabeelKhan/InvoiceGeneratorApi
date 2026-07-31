using System.ComponentModel.DataAnnotations;

namespace BusinessObjectsLayer.Entities
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "UserId must be between 5 and 100 characters.")]
        public string UserId { get; set; }
        public string Password { get; set; }
    }
    public class ChangePassword
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Current password is required.")]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        public string? NewPassword { get; set; }
    }
}
