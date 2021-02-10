using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos;

namespace Api.Interfaces
{
    public interface IInfringementRepository
    {
        Task<List<InfringementDto>> GetInfringementsAsync();
        Task<InfringementDto> GetInfringementByIdAsync(int id);
        Task<List<InfringementDto>> GetInfringementsByUserIdAsync(int userId);
        Task<InfringementDto> InsertInfringementAsync(InfringementDto infringementDto);
        Task<InfringementDto> UpdateInfringementAsync(InfringementDto infringementDto);
        Task<bool> DeleteInfringementAsync(int id);
        Task<bool> Complete();
    }
}