using System.Collections.Generic;
using System.Xml;
using Api.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api.Dtos
{
    public class FishingClubDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public JObject Rules { get; set; }
        public JObject Fish { get; set; }
        public bool IsActice { get; set; }
    }
}