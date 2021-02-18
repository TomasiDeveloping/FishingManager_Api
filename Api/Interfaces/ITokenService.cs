using Api.Dtos;
using Api.Entities;

namespace Api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}