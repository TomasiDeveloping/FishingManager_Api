using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;
using Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    public class ClubController : BaseController
    {
        private readonly IFishingClubRepository _fishingClubRepository;

        public ClubController(IFishingClubRepository fishingClubRepository)
        {
            _fishingClubRepository = fishingClubRepository;
        }

        [HttpGet]
        public async Task<ActionResult<FishingClubDto>> Get()
        {
            return Ok(await _fishingClubRepository.GetFishingClubAsync());
        }

        [HttpGet("Users")]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            return Ok(await _fishingClubRepository.GetUsersAsync());
        }

        [HttpGet("Licences")]
        public async Task<ActionResult<List<LicenceDto>>> GetLicences()
        {
            return Ok(await _fishingClubRepository.GetLicensesAsync());
        }
        
        [HttpGet("Statistics")]
        public async Task<ActionResult<List<StatisticDto>>> GetStatistics()
        {
            return Ok(await _fishingClubRepository.GetStatisticsAsync());
        }

        [HttpGet("Infringements")]
        public async Task<ActionResult<List<InfringementDto>>> GetInfringements()
        {
            return Ok(await _fishingClubRepository.GetInfringementsAsync());
        }

        [HttpGet("Rights")]
        public async Task<ActionResult<Right>> GetRights()
        {
            return Ok(await _fishingClubRepository.GetRightsAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FishingClubDto>> Put(int id, FishingClubDto fishingClubDto)
        {
            if (id <= 0) return BadRequest("ID Fehler");
            var checkUpdate = await _fishingClubRepository.UpdateAsync(fishingClubDto);
            if (checkUpdate == null) return BadRequest("Fehler beim aktualisieren");
            return Ok(checkUpdate);
        }
    }
}