using System.Threading.Tasks;
using System.Xml;
using Api.Dtos;
using Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api.Data
{
    public class FishingClubRepository : IFishingClubRepository
    {
        private readonly FishingManagerContext _fishingManagerContext;

        public FishingClubRepository(FishingManagerContext fishingManagerContext)
        {
            _fishingManagerContext = fishingManagerContext;
        }
        public async Task<FishingClubDto> GetFishingClubByIdAsync(int fishingClubId)
        {
            var club = await _fishingManagerContext.FishingClubs
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == fishingClubId);

            if (club == null) return null;

            var fishJson = new JObject();
            var rulesJson = new JObject();
            if (!string.IsNullOrEmpty(club.FishSpecies))
            {
                var xmlFish = new XmlDocument();
                xmlFish.LoadXml(club.FishSpecies);
                fishJson = JObject.Parse(JsonConvert.SerializeXmlNode(xmlFish));
            }

            if (!string.IsNullOrEmpty(club.Rules))
            {
                var xmlRules = new XmlDocument();
                xmlRules.LoadXml(club.Rules);
                rulesJson = JObject.Parse(JsonConvert.SerializeXmlNode(xmlRules));
            }
      
            return new FishingClubDto
            {
                Id = club.Id,
                Address = club.Address,
                Name = club.Name,
                Fish = fishJson,
                Rules = rulesJson,
                IsActice = club.IsActive
            };
        }
    }
}