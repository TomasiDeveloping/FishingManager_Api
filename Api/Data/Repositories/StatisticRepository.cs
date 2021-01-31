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
using Newtonsoft.Json.Linq;

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
        public async Task<List<StatisticDto>> GetStatisticsAsync()
        {
            var statistics =  await _context.Statistics
                .AsNoTracking()
                .Select(s => new 
                {
                    s.Id,
                    s.Year,
                    s.UserId,
                    s.LicenceId,
                    FullName = $"{s.User.FirstName} {s.User.LastName}",
                    Statistic = s.StatisticXml
                })
                .ToListAsync();
            var dtoList = new List<StatisticDto>();
            foreach (var item in statistics)
            {
                var xml = new XmlDocument();
                xml.LoadXml(item.Statistic);
                var jsonStatistic = JsonConvert.SerializeXmlNode(xml);
                dtoList.Add(new StatisticDto()
                {
                    Id = item.Id,
                    Year = item.Year,
                    FullName = item.FullName,
                    UserId = item.UserId,
                    LicenceId = item.LicenceId,
                    Statistic = JObject.Parse(jsonStatistic)
                });
            }

            return dtoList;
        }

        public async Task<StatisticDto> GetStatisticByIdAsync(int statisticId)
        {
            var statistic = await _context.Statistics
                .Include(s => s.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == statisticId);
            if (statistic == null) return null;
            var jsonStatistics = new JObject();
            if (!string.IsNullOrEmpty(statistic.StatisticXml))
            {
                var xml = new XmlDocument();
                xml.LoadXml(statistic.StatisticXml);
                jsonStatistics = JObject.Parse(JsonConvert.SerializeXmlNode(xml));
            }

            return new StatisticDto()
            {
                Id = statistic.Id,
                Year = statistic.Year,
                FullName = $"{statistic.User.FirstName} {statistic.User.LastName}",
                LicenceId = statistic.LicenceId,
                UserId = statistic.UserId,
                Statistic = jsonStatistics
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
            
            var nodeFirstName = xml.SelectSingleNode("Statistik/Inhaber/Vorname");
            var nodeLastName = xml.SelectSingleNode("Statistik/Inhaber/Nachname");
            var nodeYear = xml.SelectSingleNode("Statistik/Fangstatistik/Jahr/Datum");

            if (nodeFirstName == null || nodeLastName == null || nodeYear == null)
            {
                _logger.InsertDatabaseLog(new DataBaseLog()
                {
                    Type = "XML Error StatisticRepository",
                    Message = "Fehler beim Zugriff auf XML Node"
                });
                return null;
            }
            nodeFirstName.InnerText = user.FirstName;
            nodeLastName.InnerText = user.LastName;
            nodeYear.InnerText = DateTime.Now.Year.ToString();

            await _context.Statistics.AddAsync(new Statistic()
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
            _logger.InsertDatabaseLog(new DataBaseLog()
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
                _logger.InsertDatabaseLog(new DataBaseLog()
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