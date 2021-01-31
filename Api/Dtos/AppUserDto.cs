namespace Api.Dtos
{
    public class AppUserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int RightId { get; set; }
        public string Token { get; set; }
    }
}