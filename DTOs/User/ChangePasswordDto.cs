using System.ComponentModel.DataAnnotations;

namespace ShopNext.DTOs.User
{
    public class ChangePasswordDto
    {
        public required string CurrentPassword { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must have uppercase, lowercase, number, and special character")]
        public required string NewPassword { get; set; }
    }
}
