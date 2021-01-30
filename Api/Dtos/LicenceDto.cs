using System;

namespace Api.Dtos
{
    public class LicenceDto
    {
        public int LicenceId { get; set; }
        public string LicenceName { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string CreatorName { get; set; }
        public int CreatorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Paid { get; set; }
    }
}