using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos;

namespace Api.Interfaces
{
    public interface IFishingClubRepository
    {
        public Task<FishingClubDto> GetFishingClubAsync();
        public Task<List<UserDto>> GetUsersAsync();
        public Task<List<LicenceDto>> GetLicensesAsync();
        public Task<List<StatisticDto>> GetStatisticsAsync();
        public Task<List<InfringementDto>> GetInfringementsAsync();
    }
}