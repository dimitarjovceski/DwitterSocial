using DwitterSocial.Dtos;
using DwitterSocial.Entities;
using DwitterSocial.Extensions;
using DwitterSocial.Helpers;
using DwitterSocial.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace DwitterSocial.Data
{
    public class LikeRepository : ILikeRepository
    {
        private readonly AppDbContext context;

        public LikeRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<UserLikes> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = context.Likes.AsQueryable();    

            if(likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(u => u.LikedUser);
            }

            if(likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {
                Id = user.Id,
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);

            
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await context.Users
                 .Include(p => p.LikedUsers)
                 .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
