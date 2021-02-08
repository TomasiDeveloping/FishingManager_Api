using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Api.Dtos;
using Api.Entities;
using Api.Helper;
using Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Api.Data.Repositories
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly FishingManagerContext _context;
        private readonly DatabaseLogger _logger;

        public StatisticRepository(FishingManagerContext context, DatabaseLogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<StatisticDto> GetStatisticByIdAsync(int statisticId)
        {
            var statistic = await _context.Statistics
                .Include(s => s.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == statisticId);

            if (statistic == null) return null;
            var xml = new XmlDocument();
            xml.LoadXml(statistic.StatisticXml);

            var newStatistic = new CatchStatistic
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
                    newStatistic.Months.Add(newMonth);

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

            return new StatisticDto
            {
                Id = statistic.Id,
                Year = statistic.Year,
                FullName = $"{statistic.User.FirstName} {statistic.User.LastName}",
                LicenceId = statistic.LicenceId,
                UserId = statistic.UserId,
                Statistic = newStatistic
            };
        }

        public async Task<StatisticDto> InsertStatisticAsync(StatisticDto statisticDto)
        {
            var user = await _context.Users
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    UserId = u.Id
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == statisticDto.UserId);

            if (user == null) return null;

            var xml = new XmlDocument();
            xml.Load(@"Helper\Statistic.xml");

            var nodeFirstName = xml.SelectSingleNode("Statistik/Vorname");
            var nodeLastName = xml.SelectSingleNode("Statistik/Nachname");
            var nodeYear = xml.SelectSingleNode("Statistik/Jahr");

            if (nodeFirstName == null || nodeLastName == null || nodeYear == null)
            {
                _logger.InsertDatabaseLog(new DataBaseLog
                {
                    Type = "XML Error StatisticRepository",
                    Message = "Fehler beim Zugriff auf XML Node"
                });
                return null;
            }

            nodeFirstName.InnerText = user.FirstName;
            nodeLastName.InnerText = user.LastName;
            nodeYear.InnerText = DateTime.Now.Year.ToString();

            await _context.Statistics.AddAsync(new Statistic
            {
                LicenceId = statisticDto.LicenceId,
                UserId = statisticDto.UserId,
                Year = DateTime.Now.Year,
                StatisticXml = xml.OuterXml
            });

            var checkInsert = await Complete();
            return checkInsert ? statisticDto : null;
        }

        public async Task<StatisticDto> UpdateStatisticAsync(StatisticDto statisticDto)
        {
            var statisticToUpdate = await _context.Statistics.FindAsync(statisticDto.Id);
            if (statisticToUpdate == null) return null;
            statisticToUpdate.UserId = statisticDto.UserId;
            var doc = JsonConvert.DeserializeXmlNode(statisticDto.Statistic.ToString());
            statisticToUpdate.StatisticXml = doc.OuterXml;
            var checkUpdate = await Complete();
            return checkUpdate ? statisticDto : null;
        }

        public async Task<bool> DeleteStatisticAsync(int statisticId)
        {
            var statisticToDelete = await _context.Statistics.FindAsync(statisticId);
            if (statisticToDelete == null) return false;
            _context.Remove(statisticToDelete);
            var checkDelete = await Complete();
            if (!checkDelete) return false;
            _logger.InsertDatabaseLog(new DataBaseLog
            {
                Type = "Statistic gelöscht",
                Message = $"Statistic {statisticToDelete} wurde gelöscht",
                CreatedAt = DateTime.Now
            });
            return true;
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
                _logger.InsertDatabaseLog(new DataBaseLog
                {
                    Type = "Error StatisticRepository",
                    Message = e.InnerException?.Message,
                    CreatedAt = DateTime.Now
                });
                return false;
            }
        }
    }
}