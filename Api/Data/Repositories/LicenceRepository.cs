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

namespace Api.Data.Repositories
{
    public class LicenceRepository : ILicenceRepository
    {
        private readonly FishingManagerContext _context;
        private readonly DatabaseLogger _logger;

        public LicenceRepository(FishingManagerContext context, DatabaseLogger logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<LicenceDto>> GetLicencesAsync()
        {
            return await _context.Licences
                .Include(l => l.User)
                .Include(l => l.Creator)
                .Select(l => new LicenceDto()
                {
                    LicenceId = l.Id,
                    LicenceName = l.LicenseName,
                    UserId = l.UserId,
                    UserName = $"{l.User.FirstName} {l.User.LastName}",
                    CreatorId = l.CreatorId,
                    CreatorName = $"{l.Creator.FirstName} {l.Creator.LastName}",
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Paid = l.Paid,
                    Year = l.Year
                })
                .AsNoTracking()
                .OrderByDescending(l => l.Year)
                .ThenBy(l => l.EndDate)
                .ToListAsync();
        }

        public async Task<LicenceDto> GetLicenceByIdAsync(int licenceId)
        {
            return await _context.Licences
                .Include(l => l.User)
                .Include(l => l.Creator)
                .Select(l => new LicenceDto()
                {
                    LicenceId = l.Id,
                    LicenceName = l.LicenseName,
                    UserId = l.UserId,
                    UserName = $@"{l.User.FirstName} {l.User.LastName}",
                    CreatorId = l.CreatorId,
                    CreatorName = $@"{l.Creator.FirstName} {l.Creator.LastName}",
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Paid = l.Paid,
                    Year = l.Year
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LicenceId == licenceId);
        }

        public async Task<LicenceDto> InsertLicenceAsync(LicenceDto licenceDto)
        {
            var newLicence = new Licence()
            {
                UserId = licenceDto.UserId,
                CreatorId = licenceDto.CreatorId,
                LicenseName = licenceDto.LicenceName,
                StartDate = new DateTime(licenceDto.StartDate.Year, licenceDto.StartDate.Month,
                    licenceDto.StartDate.Day, 0, 0, 0),
                EndDate = new DateTime(licenceDto.EndDate.Year, licenceDto.EndDate.Month, licenceDto.EndDate.Day, 23,
                    59, 59),
                Paid = licenceDto.Paid,
                Year = licenceDto.Year
            };
            var user = await _context.Users
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    UserId = u.Id
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == licenceDto.UserId);
            var xml = new XmlDocument();
            xml.LoadXml(@"<Statistik>
                 <Fischerverein>Fischerverein Muster</Fischerverein>
                 <Vorname />
                 <Nachname />
                 <Jahr />
                 <Monate/>
                 </Statistik>");

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
            nodeYear.InnerText = licenceDto.Year.ToString();
            var newStatistic = new Statistic()
            {
                Licence = newLicence,
                UserId = licenceDto.UserId,
                Year = licenceDto.StartDate.Year,
                StatisticXml = xml.OuterXml
            };
            await _context.Statistics.AddAsync(newStatistic);
         
            var checkInsert = await Complete();

            if (!checkInsert) return null;
            
            _logger.InsertDatabaseLog(new DataBaseLog()
            {
                Type = "Neue Lizenz",
                Message = $"Neue Lizenz {licenceDto.LicenceName} wurde hinzugefügt durch {licenceDto.CreatorId}",
                CreatedAt = DateTime.Now
            });
            return licenceDto;

        }

        public async Task<LicenceDto> UpdateLicenceAsync(LicenceDto licenceDto)
        {
            var licenceToUpdate = await _context.Licences.FindAsync(licenceDto.LicenceId);
            licenceToUpdate.CreatorId = licenceDto.CreatorId;
            licenceToUpdate.UserId = licenceDto.UserId;
            licenceToUpdate.Paid = licenceDto.Paid;
            licenceToUpdate.StartDate = new DateTime(licenceDto.StartDate.Year, licenceDto.StartDate.Month,
                licenceDto.StartDate.Day, 0, 0, 0).AddDays(1);
            licenceToUpdate.EndDate = new DateTime(licenceDto.EndDate.Year, licenceDto.EndDate.Month,
                licenceDto.EndDate.Day, 23, 59, 59);
            licenceToUpdate.LicenseName = licenceDto.LicenceName;
            licenceToUpdate.Year = licenceDto.Year;

            var user = await _context.Users
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    UserId = u.Id
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == licenceDto.UserId);
            
            var statistic = await _context.Statistics
                .FirstOrDefaultAsync(s => s.LicenceId == licenceDto.LicenceId);
            statistic.UserId = licenceDto.UserId;
            statistic.Year = licenceDto.EndDate.Year;

            var xml = new XmlDocument();
            xml.LoadXml(statistic.StatisticXml);
            
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
            nodeYear.InnerText = licenceDto.StartDate.Year.ToString();
            statistic.StatisticXml = xml.OuterXml;

            var checkUpdate = await Complete();

            return checkUpdate ? licenceDto : null;
        }

        public async Task<bool> DeleteLicenceAsync(int licenceId)
        {
            var licenceToDelete = await _context.Licences.FindAsync(licenceId);
            if (licenceToDelete == null) return false;
            var statisticToDelete = await _context.Statistics.FirstOrDefaultAsync(s => s.LicenceId == licenceId);
            if (statisticToDelete == null) return false;
            _context.Remove(statisticToDelete);
            _context.Remove(licenceToDelete);
            var checkDelete = await Complete();
            if (!checkDelete) return false;
            _logger.InsertDatabaseLog(new DataBaseLog()
            {
                Type = "Lizenz gelöscht",
                Message = $"Lizenz mit der Id {licenceId} wurde gelöscht",
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
                    Type = "Error LicenceRepository",
                    Message = e.InnerException?.Message,
                    CreatedAt = DateTime.Now
                });
                return false;
            }
        }
    }
}