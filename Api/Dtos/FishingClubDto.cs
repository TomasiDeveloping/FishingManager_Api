using System.Collections.Generic;
using Api.Entities;
using Newtonsoft.Json.Linq;

namespace Api.Dtos
{
    public class FishingClubDto
    {
        public int FishingClubId { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public ICollection<Rules> Rules { get; set; }
        public ICollection<FishSpecies> FishSpecies { get; set; }
        public string Website { get; set; }
        public string PictureUrl { get; set; }
        public string ExternRuleUrl { get; set; }
    }

    public class Rules
    {
        public string Rule { get; set; }
    }

    public class FishSpecies
    {
        public string FishSpecie { get; set; }
        public string MinimumSize { get; set; }
        public string ClosedSeasonStart { get; set; }
        public string ClosedSeasonEnd { get; set; }
    }
}