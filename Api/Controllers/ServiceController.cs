using System.Threading.Tasks;
using Api.Dtos;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        [HttpPost("contact")]
        public async Task<ActionResult<bool>> SendContactMail(ContactDto contactDto)
        {
            return Ok(await _serviceRepository.SendContactMailAsync(contactDto));
        }
    }
}