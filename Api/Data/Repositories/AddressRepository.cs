using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Entities;
using Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly FishingManagerContext _context;

        public AddressRepository(FishingManagerContext context)
        {
            _context = context;
        }
        public async Task<List<Address>> GetAddressesAsync()
        {
            return await _context.Addresses
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Address> GetAddressByIdAsync(int addressId)
        {
            return await _context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == addressId);
        }

        public async Task<Address> InsertAddressAsync(Address address)
        {
            await _context.Addresses.AddAsync(address);
            var checkInsert = await Complete();
            return checkInsert ? address : null;
        }

        public async Task<Address> UpdateAddressAsync(Address address)
        {
            var addressToUpdate = await _context.Addresses.FindAsync(address.Id);
            addressToUpdate.City = address.City;
            addressToUpdate.Phone = address.Phone;
            addressToUpdate.Street = address.Street;
            addressToUpdate.Title = address.Title;
            addressToUpdate.Zip = address.Zip;
            addressToUpdate.AddressAddition = address.AddressAddition;

            var checkUpdate = await Complete();
            return checkUpdate ? address : null;
        }

        public async Task<bool> DeleteAddressAsync(int addressId)
        {
            var addressToDelete = await _context.Addresses.FindAsync(addressId);
            if (addressToDelete == null) return false;
            return await Complete();
        }

        public async Task<bool> Complete()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                await _context.DataBaseLogs.AddAsync(new DataBaseLog()
                {
                    Type = "Db Address Error",
                    Message = e.Message,
                    CreatedAt = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return false;
            }
        }
    }
}