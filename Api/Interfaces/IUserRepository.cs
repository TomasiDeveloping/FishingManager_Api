using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;


namespace Api.Interfaces
{
    public interface IUserRepository
    {
        public Task<UserDto> GetUserByIdAsync(int userId);
        public Task<List<Licence>>GetUserLicencesAsync(int userId);
        public Task<List<StatisticDto>> GetUserStatisticsAsync(int userId);
    }
}