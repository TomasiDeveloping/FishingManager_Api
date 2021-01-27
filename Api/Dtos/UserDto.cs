using Api.Entities;

namespace Api.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string RightName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PictureUrl { get; set; }
        public Address Address { get; set; }
    }
}