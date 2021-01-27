using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Api.Dtos;
using Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api.Data.Repositories
{
    public class FishingClubRepository : IFishingClubRepository
    {
        private readonly FishingManagerContext _context;

        public FishingClubRepository(FishingManagerContext context)
        {
            _context = context;
        }
        public async Task<FishingClubDto> GetFishingClubAsync()
        {
            var club = await _context.FishingClubs
                .Include(c => c.Address)
                .FirstOrDefaultAsync();

            var rulesJson = new JObject();
            var fishSpeciesJson = new JObject();

            if (!string.IsNullOrEmpty(club.Rules))
            {
                var ruleXml = new XmlDocument();
                ruleXml.LoadXml(club.Rules);
                rulesJson = JObject.Parse(JsonConvert.SerializeXmlNode(ruleXml));
            }

            if (!string.IsNullOrEmpty(club.FishSpecies))
            {
                var speciesXml = new XmlDocument();
                speciesXml.LoadXml(club.FishSpecies);
                fishSpeciesJson = JObject.Parse(JsonConvert.SerializeXmlNode(speciesXml));
            }

            return new FishingClubDto()
            {
                FishingClubId = club.Id,
                Name = club.Name,
                Address = club.Address,
                Website = club.Website,
                PictureUrl = club.PictureUrl,
                Rules = rulesJson,
                FishSpecies = fishSpeciesJson
            };
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.Right)
                .ToListAsync();

            return users.Select(u => new UserDto
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Address = u.Address,
                RightName = u.Right.Name,
                PictureUrl = u.PictureUrl,
                UserId = u.Id
            }).ToList();
        }

        public async Task<List<LicenceDto>> GetLicensesAsync()
        {
            var licences = await _context.Licences
                .Include(l => l.User)
                .Include(l => l.Creator)
                .ToListAsync();

            return licences.Select(l => new LicenceDto
            {
                UserName = $@"{l.User.FirstName} {l.User.LastName}",
                CreatorName = $@"{l.Creator.FirstName} {l.Creator.LastName}",
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Paid = l.Paid,
                LicenceId = l.Id
            }).ToList();
        }

        public async Task<List<StatisticDto>> GetStatisticsAsync()
        {
            var statistics = await _context.Statistics
                .Include(s => s.User)
                .ToListAsync();

            var dtoList = new List<StatisticDto>();

            foreach (var item in statistics)
            {
                var jsonStatistics = new JObject();
                if (!string.IsNullOrEmpty(item.StatisticXml))
                {
                    var xml = new XmlDocument();
                    xml.LoadXml(item.StatisticXml);
                    jsonStatistics = JObject.Parse(JsonConvert.SerializeXmlNode(xml));
                }
                dtoList.Add(new StatisticDto
                {
                    Id = item.Id,
                    Year = item.Year,
                    FullName = $@"{item.User.FirstName} {item.User.LastName}",
                    Statistic = jsonStatistics
                });
            }

            return dtoList;
        }

        public async Task<List<InfringementDto>> GetInfringementsAsync()
        {
            var infringements = await _context.Infringements
                .Include(i => i.User)
                .Include(i => i.Creator)
                .ToListAsync();

            return infringements.Select(i => new InfringementDto
            {
                InfringementId = i.Id,
                UserName = $@"{i.User.FirstName} {i.User.LastName}",
                CreatorName = $@"{i.Creator.FirstName} {i.Creator.LastName}",
                Description = i.Description,
                CreatedAt = i.CreatedAt
            }).ToList();
        }
    }
}