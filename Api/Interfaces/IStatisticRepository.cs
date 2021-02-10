using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;

namespace Api.Interfaces
{
    public interface IStatisticRepository
    {
        public Task<StatisticDto> GetStatisticByIdAsync(int statisticId);
        public Task<StatisticDto> GetStatisticByLicenceIdAsync(int licenceId);
        public Task<StatisticDto> InsertStatisticAsync(StatisticDto statisticDto);
        public Task<StatisticDto> UpdateStatisticAsync(StatisticDto statisticDto);
        public Task<bool> DeleteStatisticAsync(int statisticId);
        public Task<bool> Complete();
    }
}