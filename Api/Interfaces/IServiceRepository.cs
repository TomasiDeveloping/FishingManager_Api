using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;

namespace Api.Interfaces
{
    public interface IServiceRepository
    {
        public Task<bool> SendContactMailAsync(ContactDto contactDto);
        public Task<bool> SendNewPasswordMailAsync(User user);
        public Task<bool> SendNewUserMailAsync(UserDto userDto, string clearTextPassword);
    }
}