using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class LicencesController : BaseController
    {
        private readonly ILicenceRepository _licenceRepository;

        public LicencesController(ILicenceRepository licenceRepository)
        {
            _licenceRepository = licenceRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<LicenceDto>>> Get()
        {
            var licences = await _licenceRepository.GetLicencesAsync();
            if (licences.Count <= 0) return NoContent();
            return Ok(licences);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LicenceDto>> Get(int id)
        {
            if (id <= 0) return BadRequest("Fehler mit der Id");
            var licence = await _licenceRepository.GetLicenceByIdAsync(id);
            if (licence == null) return NotFound("Keine Lizenz gefunden");
            return Ok(licence);
        }

        [HttpPost]
        public async Task<ActionResult<LicenceDto>> Post(LicenceDto licenceDto)
        {
            if (licenceDto == null) return BadRequest("Keine Lizenz zum Einfügen");
            if (licenceDto.UserId <= 0 || licenceDto.CreatorId <= 0) return BadRequest("User oder Creator Fehler");

            var newLicence = await _licenceRepository.InsertLicenceAsync(licenceDto);
            if (newLicence == null) return BadRequest("Lizenz konnte nicht hinzugefügt werden");
            return Ok(newLicence);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LicenceDto>> Put(int id, LicenceDto licenceDto)
        {
            if (licenceDto == null) return BadRequest("Keine Lizenz zum Updaten");
            if (id <= 0 || licenceDto.UserId <= 0 || licenceDto.CreatorId <= 0) return BadRequest("Id Fehler");
            var updateLicence = await _licenceRepository.UpdateLicenceAsync(licenceDto);
            if (updateLicence == null) return BadRequest("Fehler beim Updaten");
            return Ok(updateLicence);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            if (id <= 0) return BadRequest("Keine Id");
            var checkDelete = await _licenceRepository.DeleteLicenceAsync(id);
            if (!checkDelete) return BadRequest("Lizenz konnte nicht gelöscht werden");
            return Ok(true);
        }
    }
}