using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;

namespace Api.Interfaces
{
    public interface IFishingClubRepository
    {
        public Task<FishingClubDto> GetFishingClubAsync();
        public Task<List<UserDto>> GetUsersAsync();
        public Task<List<LicenceDto>> GetLicensesAsync();
        public Task<List<StatisticDto>> GetStatisticsAsync();
        public Task<List<Right>> GetRightsAsync();
        public Task<List<InfringementDto>> GetInfringementsAsync();
        public Task<FishingClubDto> InsertAsync(FishingClubDto fishingClubDto);
        public Task<FishingClubDto> UpdateAsync(FishingClubDto fishingClubDto);
        public Task<bool> Complete();
    }
}