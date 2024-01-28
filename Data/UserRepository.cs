using AutoMapper;
using AutoMapper.Configuration.Annotations;
using AutoMapper.QueryableExtensions;
using DwitterSocial.Dtos;
using DwitterSocial.Entities;
using DwitterSocial.Helpers;
using DwitterSocial.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DwitterSocial.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<AppUser> userManager;

        public UserRepository(AppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<AppUser> GetByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetByUsernameAsync(string username)
        {
            return await context.Users
                 .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<MemberDto> GetMemberByUsernameAsync(string username)
        {
            return await context.Users
                     .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<string> GetMemberGender(string username)
        {
            return await context.Users.Where(x => x.UserName == username)
                .Select(x => x.Gender)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(query.
                ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .AsNoTracking(), userParams.PageNumber, userParams.PageSize);    
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await context.Users.ToListAsync();
        }

        public void Update(AppUser user)
        {
           
            context.Entry(user).State = EntityState.Modified;
        }
    }
}
