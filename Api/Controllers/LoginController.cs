using System.Threading.Tasks;
using Api.Data;
using Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class LoginController : BaseController
    {
        private readonly FishingManagerContext _context;

        public LoginController(FishingManagerContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AppUserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Password.Equals(loginDto.Password));
            if (user == null) return BadRequest("Email oder Password falsch");
            return Ok(new AppUserDto()
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                RightId = user.RightId,
                Token = "TEST_TOKEN"
            });
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (user == null) return BadRequest("Kein User mit dieser Email registriert");
            return Ok("Neues Password wird per E-Mail gesendet");
        }
    }
}