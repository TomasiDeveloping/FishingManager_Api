using Newtonsoft.Json.Linq;

namespace Api.Dtos
{
    public class StatisticDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Year { get; set; }
        public JObject Statistic { get; set; }
    }
}