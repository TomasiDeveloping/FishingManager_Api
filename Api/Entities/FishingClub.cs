using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Entities
{
    public class FishingClub
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public int AddressId { get; set; }
        public DateTime CreatedAt { get; set; }
        [Column(TypeName="xml")]
        public string Rules { get; set; }
        [Column(TypeName="xml")]
        public string FishSpecies { get; set; }
        public string Website { get; set; }
        public string PictureUrl { get; set; }
    }
}