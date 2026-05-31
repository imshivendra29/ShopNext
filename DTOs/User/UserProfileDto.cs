namespace ShopNext.DTOs.User
{
    public class UserProfileDto
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
