using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos;
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

        [HttpGet("Licences/{userId}")]
        public async Task<ActionResult<List<LicenceDto>>> GetLicencesByUserId(int userId)
        {
            return Ok(await _userRepository.GetUserLicencesAsync(userId));
        }

        [HttpGet("Statistics/{userId}")]
        public async Task<ActionResult<List<StatisticDto>>> GetStatisticsByUserId(int userId)
        {
            return Ok(await _userRepository.GetUserStatisticsAsync(userId));
        }
    }
}