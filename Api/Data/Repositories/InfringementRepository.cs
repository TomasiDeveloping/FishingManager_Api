using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;
using Api.Helper;
using Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repositories
{
    public class InfringementRepository : IInfringementRepository
    {
        private readonly FishingManagerContext _context;
        private readonly DatabaseLogger _logger;

        public InfringementRepository(FishingManagerContext context, DatabaseLogger logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<InfringementDto>> GetInfringementsAsync()
        {
            return await _context.Infringements
                .AsNoTracking()
                .Include(i => i.Creator)
                .Include(i => i.User)
                .Select(i => new InfringementDto()
                {
                    InfringementId = i.Id,
                    UserId = i.UserId,
                    UserName = $"{i.User.FirstName} {i.User.LastName}",
                    CreatorId = i.CreatorId,
                    CreatorName = $"{i.Creator.FirstName} {i.Creator.LastName}",
                    CreatedAt = i.CreatedAt,
                    Description = i.Description
                })
                .ToListAsync();
        }

        public async Task<InfringementDto> GetInfringementByIdAsync(int id)
        {
            return await _context.Infringements
                .Include(i => i.User)
                .Include(i => i.Creator)
                .Select(i => new InfringementDto()
                {
                    InfringementId = i.Id,
                    UserId = i.UserId,
                    UserName = $"{i.User.FirstName} {i.User.LastName}",
                    CreatorId = i.CreatorId,
                    CreatorName = $"{i.Creator.FirstName} {i.Creator.LastName}",
                    CreatedAt = i.CreatedAt,
                    Description = i.Description
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InfringementId == id);
        }

        public async Task<List<InfringementDto>> GetInfringementsByUserIdAsync(int userId)
        {
            return await _context.Infringements
                .Include(i => i.User)
                .Include(i => i.Creator)
                .Select(i => new InfringementDto()
                {
                    InfringementId = i.Id,
                    UserId = i.UserId,
                    UserName = $"{i.User.FirstName} {i.User.LastName}",
                    CreatorId = i.CreatorId,
                    CreatorName = $"{i.Creator.FirstName} {i.Creator.LastName}",
                    CreatedAt = i.CreatedAt,
                    Description = i.Description
                })
                .AsNoTracking()
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        public async Task<InfringementDto> InsertInfringementAsync(InfringementDto infringementDto)
        {
            if (infringementDto == null) return null;
            var infringement = new Infringement()
            {
                UserId = infringementDto.UserId,
                CreatorId = infringementDto.CreatorId,
                Description = infringementDto.Description,
                CreatedAt = DateTime.Now
            };

            await _context.Infringements.AddAsync(infringement);
            var checkInsert = await Complete();
            return checkInsert ? infringementDto : null;
        }
        
        public async Task<InfringementDto> UpdateInfringementAsync(InfringementDto infringementDto)
        {
            var infringement = await _context.Infringements.FindAsync(infringementDto.InfringementId);
            if (infringement == null) return null;
            infringement.Description = infringementDto.Description;
            infringement.CreatorId = infringementDto.CreatorId;
            infringement.UserId = infringementDto.UserId;
            var checkUpdate = await Complete();
            return checkUpdate ? infringementDto : null;
        }

        public async Task<bool> DeleteInfringementAsync(int id)
        {
            var toDelete = await _context.Infringements.FindAsync(id);
            if (toDelete == null) return false;
            _context.Remove(toDelete);
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
                var log = new DataBaseLog()
                {
                    Type = "Error Infringement Repository",
                    Message = e.InnerException?.Message,
                    CreatedAt = DateTime.Now
                };
                _logger.InsertDatabaseLog(log);
                return false;
            }
        }
    }
}