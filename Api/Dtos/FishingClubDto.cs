using Api.Entities;
using Newtonsoft.Json.Linq;

namespace Api.Dtos
{
    public class FishingClubDto
    {
        public int FishingClubId { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public JObject Rules { get; set; }
        public JObject FishSpecies { get; set; }
        public string Website { get; set; }
        public string PictureUrl { get; set; }
    }
}