using System;

namespace Api.Dtos
{
    public class InfringementDto
    {
        public int InfringementId { get; set; }
        public string UserName { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
    }
}