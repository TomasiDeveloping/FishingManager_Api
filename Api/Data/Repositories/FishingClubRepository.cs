using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Api.Dtos;
using Api.Entities;
using Api.Helper;
using Api.Interfaces;
using Microsoft.EntityFrameworkCore;



namespace Api.Data.Repositories
{
    public class FishingClubRepository : IFishingClubRepository
    {
        private readonly FishingManagerContext _context;
        private readonly DatabaseLogger _logger;

        public FishingClubRepository(FishingManagerContext context, DatabaseLogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<FishingClubDto> GetFishingClubAsync()
        {
            var club = await _context.FishingClubs
                .Include(c => c.Address)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var clubDto = new FishingClubDto()
            {
                Address = club.Address,
                Name = club.Name,
                Website = club.Website,
                PictureUrl = club.PictureUrl,
                FishingClubId = club.Id,
                Rules = new List<Rules>(),
                FishSpecies = new List<FishSpecies>()
            };
            var ruleDoc = new XmlDocument();
            ruleDoc.LoadXml(club.Rules);
            var rules = ruleDoc.SelectNodes("Regeln/Regel");
            if (rules != null)
            {
                foreach (XmlNode rule in rules)
                {
                    clubDto.Rules.Add(new Rules()
                    {
                        Rule = rule.InnerText
                    });
                } 
            }

            var fishSpecieDoc = new XmlDocument();
            fishSpecieDoc.LoadXml(club.FishSpecies);
            var species = fishSpecieDoc.SelectNodes("FischArten/Fisch");
            if (species != null)
            {
                foreach (XmlNode specie in species)
                {
                    clubDto.FishSpecies.Add(new FishSpecies()
                    {
                        FishSpecie = specie.SelectSingleNode("Name")?.InnerText,
                        MinimumSize = specie.SelectSingleNode("Schonmass")?.InnerText,
                        ClosedSeasonStart = specie.SelectSingleNode("SchonZeitVon")?.InnerText,
                        ClosedSeasonEnd = specie.SelectSingleNode("SchonZeitBis")?.InnerText
                    });
                }
            }
            return clubDto;
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
                .Include(s => s.Licence)
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
                dto.LicenceId = item.Licence.Id;
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

        public async Task<FishingClubDto> UpdateAsync(FishingClubDto fishingClubDto)
        {
            var club = await _context.FishingClubs.FindAsync(fishingClubDto.FishingClubId);
            if (club == null) return null;
            club.Address = fishingClubDto.Address;
            club.Name = fishingClubDto.Name;
            club.Website = fishingClubDto.Website;
            club.PictureUrl = fishingClubDto.PictureUrl;

            var ruleXml = new XDocument();
            var rule = new XElement("Regeln");
            ruleXml.Add(rule);
            foreach (var item in fishingClubDto.Rules)
            {
                rule.Add(new XElement("Regel", item.Rule));
            }

            club.Rules = ruleXml.ToString();
            
            var fishSpecieXml = new XDocument();
            var fishSpecie = new XElement("FischArten");
            fishSpecieXml.Add(fishSpecie);
            foreach (var item in fishingClubDto.FishSpecies)
            {
                var fish = new XElement("Fisch");
                fish.Add(new XElement("Name", item.FishSpecie));
                fish.Add(new XElement("Schonmass", item.MinimumSize));
                fish.Add(new XElement("SchonZeitVon", item.ClosedSeasonStart));
                fish.Add(new XElement("SchonZeitBis", item.ClosedSeasonEnd));
                fishSpecie.Add(fish);
            }

            club.FishSpecies = fishSpecieXml.ToString();
            
            var checkUpdate = await Complete();
            return checkUpdate ? fishingClubDto : null;
        }

        public async Task<bool> Complete()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.InsertDatabaseLog(new DataBaseLog()
                {
                    Type = "Error FishingClubRepository",
                    Message = e.InnerException?.Message,
                    CreatedAt = DateTime.Now
                });
                return false;
            }
        }
    }
}