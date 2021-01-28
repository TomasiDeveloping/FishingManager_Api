using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Entities
{
    public class Infringement
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public User Creator { get; set; }
        [ForeignKey("CreatorId")]
        public int CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
    }
}