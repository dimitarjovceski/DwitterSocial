using DwitterSocial.Dtos;
using DwitterSocial.Entities;
using DwitterSocial.Helpers;

namespace DwitterSocial.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetByIdAsync(int id);
        Task<AppUser> GetByUsernameAsync(string username);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberByUsernameAsync(string username);
        Task<string> GetMemberGender(string username);
        void Update(AppUser user);

    }
}
