using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Api.Data;
using Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api.Controllers
{
    public class TestController : BaseController
    {
        private readonly FishingManagerContext _context;

        public TestController(FishingManagerContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> Get(int userId)
        {
            var test = await _context.Statistics
                .Include("User")
                .Include("Club")
                .FirstOrDefaultAsync(x => x.User.Id == userId);
            if (test == null) return NotFound($@"Keine Statistik vorhanden für den User");
            var xmlStatistic = new XmlDocument();
            xmlStatistic.LoadXml(test.Message);
            var statisticJson = JObject.Parse(JsonConvert.SerializeXmlNode(xmlStatistic));

            var statistic = new StatisticDto
            {
                Id = test.Id,
                FullName = test.User.FirstName + " " + test.User.LastName,
                FishingClub = test.Club.Name,
                Year = test.Year,
                Statistic = statisticJson
            };

            return Ok(statistic);
        }

        [HttpGet("club/{fishingClubId}")]
        public async Task<ActionResult> GetClub(int fishingClubId)
        {
            var club = await _context.FishingClubs
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Id == fishingClubId);

            var ruleXml = new XmlDocument();
            ruleXml.LoadXml(club.Rules);
            var ruleJson = JObject.Parse(JsonConvert.SerializeXmlNode(ruleXml));

            var speciesXml = new XmlDocument();
            speciesXml.LoadXml(club.FishSpecies);
            var speciesJson = JObject.Parse(JsonConvert.SerializeXmlNode(speciesXml));

            return Ok(new FishingClubDto
            {
                Id = club.Id,
                Address = club.Address,
                Fish = speciesJson,
                Name = club.Name,
                Rules = ruleJson,
                IsActice = club.IsActive
            });

        }

        [HttpGet("UserClubs/{userId}")]
        public async Task<ActionResult> GetUserClubs(int userId)
        {
            return Ok(
                await _context.Users
                    .Include(x => x.UserFishingClubs)
                    .ThenInclude(c => c.FishingClub)
                    .Select(c => new
                    {
                        UserId = c.Id,
                        UserName = $@"{c.FirstName} {c.LastName}",
                        FischerClubs = c.UserFishingClubs.Select(x => x.FishingClub.Name)
                    })
                    .FirstOrDefaultAsync(u => u.UserId == userId)
                );
        }
    }
}