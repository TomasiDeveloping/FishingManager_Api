using System;

namespace Api.Entities
{
    public class DataBaseLog
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}