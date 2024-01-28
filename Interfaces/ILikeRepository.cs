using DwitterSocial.Dtos;
using DwitterSocial.Entities;
using DwitterSocial.Helpers;

namespace DwitterSocial.Interfaces
{
    public interface ILikeRepository
    {
        Task<UserLikes> GetUserLike(int sourceId, int likedUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}
