using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;
using Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repositories
{
    public class LicenceRepository : ILicenceRepository
    {
        private readonly FishingManagerContext _context;

        public LicenceRepository(FishingManagerContext context)
        {
            _context = context;
        }
        public async Task<List<LicenceDto>> GetLicencesAsync()
        {
            var licences = await _context.Licences
                .Include(l => l.User)
                .Include(l => l.Creator)
                .ToListAsync();
            return licences.Select(l => new LicenceDto()
            {
                LicenceId = l.Id,
                LicenceName = l.LicenseName,
                UserName = $@"{l.User.FirstName} {l.User.LastName}",
                CreatorName = $@"{l.Creator.FirstName} {l.Creator.LastName}",
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Paid = l.Paid
            }).ToList();
        }

        public async Task<LicenceDto> GetLicenceByIdAsync(int licenceId)
        {
            return await _context.Licences
                .Include(l => l.User)
                .Include(l => l.Creator)
                .Select(l => new LicenceDto()
                {
                    LicenceId = l.Id,
                    LicenceName = l.LicenseName,
                    UserName = $@"{l.User.FirstName} {l.User.LastName}",
                    CreatorName = $@"{l.Creator.FirstName} {l.Creator.LastName}",
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Paid = l.Paid
                }).FirstOrDefaultAsync(l => l.LicenceId == licenceId);
        }

        public async Task<LicenceDto> InsertLicenceAsync(LicenceDto licenceDto)
        {
            var user = await _context.Users.FindAsync(licenceDto.UserId);
            var creator = await _context.Users.FindAsync(licenceDto.CreatorId);
            await _context.Licences.AddAsync(new Licence()
            {
                User = user,
                Creator = creator,
                LicenseName = licenceDto.LicenceName,
                StartDate = licenceDto.StartDate,
                EndDate = licenceDto.EndDate,
                Paid = licenceDto.Paid
            });
            var checkInsert = await Complete();

            return checkInsert ? licenceDto : null;
        }

        public async Task<LicenceDto> UpdateLicenceAsync(LicenceDto licenceDto)
        {
            var user = await _context.Users.FindAsync(licenceDto.UserId);
            var creator = await _context.Users.FindAsync(licenceDto.CreatorId);
            var licenceToUpdate = await _context.Licences.FindAsync(licenceDto.LicenceId);
            licenceToUpdate.Creator = creator;
            licenceToUpdate.User = user;
            licenceToUpdate.Paid = licenceDto.Paid;
            licenceToUpdate.StartDate = licenceDto.StartDate;
            licenceToUpdate.EndDate = licenceDto.EndDate;
            licenceToUpdate.LicenseName = licenceDto.LicenceName;

            var checkUpdate = await Complete();

            return checkUpdate ? licenceDto : null;
        }

        public async Task<bool> DeleteLicenceAsync(int licenceId)
        {
            var licenceToDelete = await _context.Licences.FindAsync(licenceId);
            if (licenceToDelete == null) return false;
            _context.Remove(licenceToDelete);
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
                    Type = "Error",
                    Message = e.Message,
                    CreatedAt = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return false;
            }
        }
    }
}