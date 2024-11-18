namespace CouponsSystem.DTO
{
    public class AdminUserCreationDto
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

}
