using System;
using System.Collections;
using System.Collections.Generic;

namespace Api.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public Right Right { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public Address Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Licence> Licences { get; set; }
        public ICollection<UserFishingClub> UserFishingClubs { get; set; }
    }
}