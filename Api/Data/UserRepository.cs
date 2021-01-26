using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Api.Dtos;
using Api.Entities;
using Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace Api.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly FishingManagerContext _context;

        public UserRepository(FishingManagerContext context)
        {
            _context = context;
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = _context.Users
                .Select(u => new UserDto
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    RightName = u.Right.Name,
                    Address = u.Address
                });
            return await user.FirstOrDefaultAsync(u => u.UserId == userId);

        }
        
        public async Task<List<Licence>> GetUserLicencesAsync(int userId)
        {
            return await _context.Licences
                .Where(u => u.User.Id == userId)
                .ToListAsync();
        }

        public async Task<List<StatisticDto>> GetUserStatisticsAsync(int userId)
        {
            var userStatistics = await _context.Statistics
                .Include(x => x.User)
                .Include(x => x.Club)
                .Where(u => u.User.Id == userId).ToListAsync();
            var dtoList = new List<StatisticDto>();

            foreach (var item in userStatistics)
            {
                var xml = new XmlDocument();
                xml.LoadXml(item.Message);
                var json = JObject.Parse(JsonConvert.SerializeXmlNode(xml));
                dtoList.Add(new StatisticDto
                {
                    Id = item.Id,
                    Year = item.Year,
                    FullName = $@"{item.User.FirstName} {item.User.LastName}",
                    FishingClub = item.Club.Name,
                    Statistic = json
                });
            }

            return dtoList;
        }
    }
}