using System.Threading.Tasks;
using Api.Data;
using Api.Dtos;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class LoginController : BaseController
    {
        private readonly FishingManagerContext _context;
        private readonly ITokenService _tokenService;

        public LoginController(FishingManagerContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<ActionResult<AppUserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Password.Equals(loginDto.Password));
            if (user == null) return BadRequest("Email oder Password falsch");
            if (!user.Active)
                return BadRequest("Sie wurden von der Plattform gesperrt ! Bitte wenden Sie sich an den Administrator");
            return Ok(new AppUserDto()
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                RightId = user.RightId,
                Token = _tokenService.CreateToken(user)
            });
        }

        [HttpGet("ForgotPassword")]
        public async Task<ActionResult<bool>> ForgotPassword([FromQuery] string email)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.Equals(email));
            return Ok(user != null);
        }
    }
}