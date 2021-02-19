using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    public class InfringementsController : BaseController
    {
        private readonly IInfringementRepository _infringementRepository;

        public InfringementsController(IInfringementRepository infringementRepository)
        {
            _infringementRepository = infringementRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<InfringementDto>>> Get()
        {
            return Ok(await _infringementRepository.GetInfringementsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InfringementDto>> Get(int id)
        {
            var infringement = await _infringementRepository.GetInfringementByIdAsync(id);

            if (infringement == null) return NotFound("Kein Verstoss gefunden mit dieser Id");
            return Ok(infringement);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<InfringementDto>>> GetByUserId(int userId)
        {
            var infringement = await _infringementRepository.GetInfringementsByUserIdAsync(userId);

            if (infringement == null) return NotFound("Kein Verstoss für diesen User");
            return Ok(infringement);
        }

        [HttpPost]
        public async Task<ActionResult<InfringementDto>> Post(InfringementDto infringementDto)
        {
            var checkInsert = await _infringementRepository.InsertInfringementAsync(infringementDto);
            if (checkInsert == null) return BadRequest("Vertsoss konnte nicht gespeichert werden");
            return Ok(infringementDto);
        }
    }
}