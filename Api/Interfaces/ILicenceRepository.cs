using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;

namespace Api.Interfaces
{
    public interface ILicenceRepository
    {
        public Task<List<LicenceDto>> GetLicencesAsync();
        public Task<LicenceDto> GetLicenceByIdAsync(int licenceId);
        public Task<LicenceDto> InsertLicenceAsync(LicenceDto licenceDto);
        public Task<LicenceDto> UpdateLicenceAsync(LicenceDto licenceDto);
        public Task<bool> DeleteLicenceAsync(int licenceId);
        public Task<bool> Complete();
    }
}