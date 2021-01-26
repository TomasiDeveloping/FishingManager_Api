using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(int id)
        {
            return Ok(await _userRepository.GetUserByIdAsync(id));
        }
        
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<List<Licence>>> Licence(int id)
        {
            return Ok(await _userRepository.GetUserLicencesAsync(id));
        }

        [HttpGet("Statistics/{userId}")]
        public async Task<ActionResult<StatisticDto>> GetUserStatistics(int userId)
        {
            return Ok(await _userRepository.GetUserStatisticsAsync(userId));
        }
    }
}