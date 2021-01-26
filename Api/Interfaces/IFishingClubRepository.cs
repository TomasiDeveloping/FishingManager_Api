using System.Threading.Tasks;
using Api.Dtos;

namespace Api.Interfaces
{
    public interface IFishingClubRepository
    {
        public Task<FishingClubDto> GetFishingClubByIdAsync(int fishingClubId);
    }
}