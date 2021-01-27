using System;

namespace Api.Entities
{
    public class Infringement
    {
        public int Id { get; set; }
        public User User { get; set; }
        public User Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
    }
}