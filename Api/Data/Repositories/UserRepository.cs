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
            var user = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.Right)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return new UserDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PictureUrl = user.PictureUrl,
                RightName = user.Right.Name
            };
        }

        public async Task<List<LicenceDto>> GetUserLicencesAsync(int userId)
        {
            var licences = await _context.Licences
                .Include(l => l.User)
                .Include(l => l.Creator)
                .Where(l => l.User.Id == userId)
                .ToListAsync();
            return licences.Select(licence => new LicenceDto
            {
                Paid = licence.Paid,
                CreatorName = @$"{licence.Creator.FirstName} {licence.Creator.LastName}",
                UserName = $@"{licence.User.FirstName} {licence.User.LastName}",
                StartDate = licence.StartDate,
                EndDate = licence.EndDate,
                LicenceId = licence.Id
            }).ToList();
        }

        public async Task<List<StatisticDto>> GetUserStatisticsAsync(int userId)
        {
            var statistics = await _context.Statistics
                .Include(s => s.User)
                .Where(s => s.User.Id == userId)
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