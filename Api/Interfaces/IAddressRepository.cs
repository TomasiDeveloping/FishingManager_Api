using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Entities;

namespace Api.Interfaces
{
    public interface IAddressRepository
    {
        public Task<List<Address>> GetAddressesAsync();
        public Task<Address> GetAddressByIdAsync(int addressId);
        public Task<Address> InsertAddressAsync(Address address);
        public Task<Address> UpdateAddressAsync(Address address);
        public Task<bool> DeleteAddressAsync(int addressId);
        public Task<bool> Complete();
    }
}