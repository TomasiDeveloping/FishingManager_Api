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
                .AsNoTracking()
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
            return await _context.Users
                .Include(u => u.Address)
                .Include(u => u.Right)
                .Select(u => new UserDto()
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    Email = u.Email,
                    LastName = u.LastName,
                    PictureUrl = u.PictureUrl,
                    RightName = u.Right.Name,
                    Address = u.Address
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<LicenceDto>> GetLicensesAsync()
        {
            return await _context.Licences
                .Include(l => l.User)
                .Include(l => l.Creator)
                .Select(l => new LicenceDto()
                {
                    LicenceId = l.Id,
                    CreatorId = l.CreatorId,
                    CreatorName = $"{l.Creator.FirstName} {l.Creator.LastName}",
                    UserId = l.UserId,
                    UserName = $"{l.User.FirstName} {l.User.LastName}",
                    LicenceName = l.LicenseName,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Paid = l.Paid
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<StatisticDto>> GetStatisticsAsync()
        {
            var statistics = await _context.Statistics
                .Include(s => s.User)
                .AsNoTracking()
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
            return await _context.Infringements
                .Include(i => i.User)
                .Include(i => i.Creator)
                .Select(i => new InfringementDto()
                {
                    InfringementId = i.Id,
                    CreatorName = $"{i.Creator.FirstName} {i.Creator.LastName}",
                    UserName = $"{i.User.FirstName} {i.User.LastName}",
                    Description = i.Description,
                    CreatedAt = i.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}