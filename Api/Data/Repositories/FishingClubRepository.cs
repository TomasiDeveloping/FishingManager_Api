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
                    RightId = u.RightId,
                    Active = u.Active,
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
                var dto = new StatisticDto();
                var xml = new XmlDocument();
                xml.LoadXml(item.StatisticXml);

                dto.Id = item.Id;
                dto.Year = item.Year;
                dto.FullName = $"{item.User.FirstName} {item.User.LastName}";
                dto.UserId = item.UserId;
                dto.LicenceName = item.Licence.LicenseName;
                var statistic = new CatchStatistic
                {
                    FishingClub = xml.SelectSingleNode("Statistik/Fischerverein")?.InnerText,
                    Year = xml.SelectSingleNode("Statistik/Jahr")?.InnerText,
                    FirstName = xml.SelectSingleNode("Statistik/Vorname")?.InnerText,
                    LastName = xml.SelectSingleNode("Statistik/Nachname")?.InnerText,
                    Months = new List<Months>()
                };

                var months = xml.SelectNodes("Statistik/Monate");
                if (months != null)
                    foreach (XmlNode month in months)
                    {
                        if (month.HasChildNodes == false) continue;
                        var newMonth = new Months
                        {
                            Month = month.SelectSingleNode("Monat")?.InnerText, Days = new List<Days>()
                        };
                        statistic.Months.Add(newMonth);

                        var days = month.SelectNodes("Tage");
                        if (days == null) continue;
                        foreach (XmlNode day in days)
                        {
                            if (day.HasChildNodes == false) continue;
                            var newTag = new Days
                            {
                                Day = day.SelectSingleNode("Tag")?.InnerText,
                                Hour = day.SelectSingleNode("Stunden")?.InnerText,
                                FishCatches = new List<FishCatch>()
                            };
                            newMonth.Days.Add(newTag);

                            var fishCatches = day.SelectNodes("Fang");
                            if (fishCatches == null) continue;
                            foreach (XmlNode fishCatch in fishCatches)
                            {
                                if (fishCatch.HasChildNodes == false) continue;
                                var newFang = new FishCatch
                                {
                                    Number = fishCatch.SelectSingleNode("Anzahl")?.InnerText,
                                    Fish = fishCatch.SelectSingleNode("Fisch")?.InnerText
                                };
                                newTag.FishCatches.Add(newFang);
                            }
                        }
                    }

                dto.Statistic = statistic;
                dtoList.Add(dto);
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