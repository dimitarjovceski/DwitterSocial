using DwitterSocial.Entities;

namespace DwitterSocial.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
