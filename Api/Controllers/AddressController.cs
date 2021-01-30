using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Entities;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class AddressController : BaseController
    {
        private readonly IAddressRepository _addressRepository;

        public AddressController(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Address>>> Get()
        {
            return Ok(await _addressRepository.GetAddressesAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> Get(int id)
        {
            var address = await _addressRepository.GetAddressByIdAsync(id);
            if (address == null) return NotFound("Keine Adresse gefunden");
            return Ok(address);
        }

        [HttpPost]
        public async Task<ActionResult<Address>> Post(Address address)
        {
            var newAddress = await _addressRepository.InsertAddressAsync(address);
            if (newAddress == null) return BadRequest("Adresse konnte nicht hinzugefügt werden");
            return Ok(newAddress);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Address>> Put(int id, Address address)
        {
            var checkAddress = await _addressRepository.GetAddressByIdAsync(id);
            if (checkAddress == null) return BadRequest("Addresse nicht gefunden");
            var updateAddress = await _addressRepository.UpdateAddressAsync(address);
            if (updateAddress == null) return BadRequest("Adresse konnte nicht geupdatet werden");
            return Ok(updateAddress);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _addressRepository.DeleteAddressAsync(id));
        }
    }
}