namespace ShopNext.DTOs.User
{
    public class UpdateProfileDto
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
