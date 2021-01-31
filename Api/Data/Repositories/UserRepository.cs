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
    public class UserRepository : IUserRepository
    {
        private readonly FishingManagerContext _context;
        private readonly DatabaseLogger _logger;

        public UserRepository(FishingManagerContext context, DatabaseLogger logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Address)
                .Include(u => u.Right)
                .Select(u => new UserDto()
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RightId = u.RightId,
                    Active = u.Active,
                    RightName = u.Right.Name,
                    PictureUrl = u.PictureUrl,
                    Address = u.Address
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<List<LicenceDto>> GetUserLicencesAsync(int userId)
        {
            return await _context.Licences
                .Include(l => l.User)
                .Include(l => l.Creator)
                .Where(l => l.User.Id == userId)
                .Select(l => new LicenceDto()
                {
                    Paid = l.Paid,
                    CreatorId = l.CreatorId,
                    CreatorName = $"{l.Creator.FirstName} {l.Creator.LastName}",
                    UserName = $"{l.User.FirstName} {l.User.LastName}",
                    EndDate = l.EndDate,
                    StartDate = l.StartDate,
                    LicenceName = l.LicenseName,
                    LicenceId = l.Id,
                    UserId = l.UserId
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<StatisticDto>> GetUserStatisticsAsync(int userId)
        {
            var statistics = await _context.Statistics
                .Include(s => s.User)
                .Where(s => s.User.Id == userId)
                .AsNoTracking()
                .ToListAsync();

            var dtoList = new List<StatisticDto>();

            foreach (var item in statistics)
            {
                var jsonStatistic = new JObject();

                if (!string.IsNullOrEmpty(item.StatisticXml))
                {
                    var xml = new XmlDocument();
                    xml.LoadXml(item.StatisticXml);
                    jsonStatistic = JObject.Parse(JsonConvert.SerializeXmlNode(xml));
                }
                dtoList.Add(new StatisticDto
                {
                    
                    Id = item.Id,
                    Year = item.Year,
                    FullName = $@"{item.User.FirstName} {item.User.LastName}",
                    Statistic = jsonStatistic
                });
            }

            return dtoList;
        }

        public async Task<UserDto> InsertUserAsync(UserDto userDto)
        {
            await _context.AddAsync(new User()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                RightId = userDto.RightId,
                PictureUrl = userDto.PictureUrl,
                Active = userDto.Active,
                CreatedAt = DateTime.Now,
                Password = "Welcome",
                Address = userDto.Address
            });
            var checkInsert = await Complete();
            if (!checkInsert) return null;
            _logger.InsertDatabaseLog(new DataBaseLog()
            {
                Type = "Neuer User",
                Message = $"Neuer User {userDto.FirstName} {userDto.LastName} wurde hinzugefügt",
                CreatedAt = DateTime.Now
            });
            return userDto;
        }

        public async Task<UserDto> UpdateUserAsync(UserDto userDto)
        {
            var userToUpdate = await _context.Users.FindAsync(userDto.UserId);
            userToUpdate.FirstName = userDto.FirstName;
            userToUpdate.LastName = userDto.LastName;
            userToUpdate.Email = userDto.Email;
            userToUpdate.PictureUrl = userDto.PictureUrl;
            userToUpdate.Active = userDto.Active;
            userToUpdate.RightId = userDto.RightId;

            var checkUpdate = await Complete();

            return checkUpdate ? userDto : null;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var userToDelete = await _context.Users.FindAsync(userId);
            if (userToDelete == null) return false;
            _context.Users.Remove(userToDelete);

            var checkDelete = await Complete();
            if (!checkDelete) return false;
            _logger.InsertDatabaseLog(new DataBaseLog()
            {
                Type = "User gelöscht",
                Message = $"User mit der Id {userId} wurde gelöscht",
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
                    Type = "Error UserRepository",
                    Message = e.InnerException?.Message,
                    CreatedAt = DateTime.Now
                });
                return false;
            }
        }
    }
}