﻿using System;
using System.Collections.Generic;

namespace Api.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public Right Right { get; set; }
        public int RightId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public Address Address { get; set; }
        public int AddressId { get; set; }
        public bool Active { get; set; }
        public string PictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserFlag { get; set; }
        public ICollection<Licence> Licences { get; set; }
    }
}