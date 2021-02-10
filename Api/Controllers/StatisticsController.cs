using System.Threading.Tasks;
using Api.Dtos;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class StatisticsController : BaseController
    {
        private readonly IStatisticRepository _statisticRepository;

        public StatisticsController(IStatisticRepository statisticRepository)
        {
            _statisticRepository = statisticRepository;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<StatisticDto>> Get(int id)
        {
            if (id <= 0) return BadRequest("Id Fehler");
            var statistic = await _statisticRepository.GetStatisticByIdAsync(id);
            if (statistic == null) return NotFound("Keine Statistik gefunden");
            return Ok(statistic);
        }

        [HttpGet("licence/{licenceId}")]
        public async Task<ActionResult<StatisticDto>> GetByLicenceId(int licenceId)
        {
            if (licenceId <= 0) return BadRequest("LicenceId fehler");
            var statistic = await _statisticRepository.GetStatisticByLicenceIdAsync(licenceId);
            if (statistic == null) return NotFound("Keine Statistik gefunden");
            return Ok(statistic);
        }

        [HttpPost]
        public async Task<ActionResult<StatisticDto>> Post(StatisticDto statisticDto)
        {
            if (statisticDto == null) return BadRequest("Fehler: Keine Daten");
            var checkInsert = await _statisticRepository.InsertStatisticAsync(statisticDto);
            if (checkInsert == null) return BadRequest("Statistik konnte nicht hinzugefügt werden");
            return Ok(checkInsert);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<StatisticDto>> Put(int id, StatisticDto statisticDto)
        {
            var statistic = await _statisticRepository.GetStatisticByIdAsync(id);
            if (statistic == null) return BadRequest("Statistik zum Updaten nicht vorhanden");
            var checkUpdate = await _statisticRepository.UpdateStatisticAsync(statisticDto);
        
            if (checkUpdate == null) return BadRequest("Fehler: Statistik konnte nicht geupdatet werden");
            return Ok(checkUpdate);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var checkDelete = await _statisticRepository.DeleteStatisticAsync(id);
            if (!checkDelete) return BadRequest("Fehler: Statistik konnte nicht gelöscht werden");
            return Ok(true);
        }
    }
}