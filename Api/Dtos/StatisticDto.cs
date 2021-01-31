using Newtonsoft.Json.Linq;

namespace Api.Dtos
{
    public class StatisticDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int Year { get; set; }
        public int LicenceId { get; set; }
        public JObject Statistic { get; set; }
    }
}