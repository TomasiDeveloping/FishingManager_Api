using System;

namespace Api.Entities
{
    public class Licence
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public User Creator { get; set; }
        public int CreatorId { get; set; }
        public string LicenseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Paid { get; set; }
    }
}