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
    public class UserRepository : IUserRepository
    {
        private readonly FishingManagerContext _context;

        public UserRepository(FishingManagerContext context)
        {
            _context = context;
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
    }
}