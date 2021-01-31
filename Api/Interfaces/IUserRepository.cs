using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos;


namespace Api.Interfaces
{
    public interface IUserRepository
    {
        public Task<UserDto> GetUserByIdAsync(int userId);
        public Task<List<LicenceDto>>GetUserLicencesAsync(int userId);
        public Task<List<StatisticDto>> GetUserStatisticsAsync(int userId);
        public Task<UserDto> InsertUserAsync(UserDto userDto);
        public Task<UserDto> UpdateUserAsync(UserDto userDto);
        public Task<bool> DeleteUserAsync(int userId);
        public Task<bool> Complete();
    }
}