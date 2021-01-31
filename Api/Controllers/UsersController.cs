﻿using System.Collections.Generic;
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
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound("Kein User gefunden");
            return Ok(user);
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

        [HttpPost]
        public async Task<ActionResult<UserDto>> Post(UserDto userDto)
        {
            if (userDto == null) return BadRequest("Kein User zum hinzufügen");
            var newUser = await _userRepository.InsertUserAsync(userDto);
            if (newUser == null) return BadRequest("User konnte nicht hinzugefügt werden");
            return Ok(newUser);
        }
    }
}