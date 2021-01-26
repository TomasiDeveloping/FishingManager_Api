using System.Threading.Tasks;
using Api.Dtos;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class FishingClubController : BaseController
    {
        private readonly IFishingClubRepository _fishingClubRepository;

        public FishingClubController(IFishingClubRepository fishingClubRepository)
        {
            _fishingClubRepository = fishingClubRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FishingClubDto>> Get(int id)
        {
            if (id <= 0) return BadRequest("Fehler bei der Id !");
            var fishingClub = await _fishingClubRepository.GetFishingClubByIdAsync(id);
            if (fishingClub == null) return NotFound("Kein Fischerverein gefunden");
            if (!fishingClub.IsActice) return BadRequest("Fischerverein ist nicht aktiv !");
            return Ok(fishingClub);
        }
    }
}