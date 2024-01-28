using AutoMapper;
using DwitterSocial.Data;
using DwitterSocial.Dtos;
using DwitterSocial.Entities;
using DwitterSocial.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DwitterSocial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService tokenService;
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IMapper mapper;

        public AccountController(ITokenService tokenService,
            SignInManager<AppUser> signInManager, UserManager<AppUser>userManager,
            IMapper mapper)
        {
            this.tokenService = tokenService;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto register)
        {
            if (await UserExist(register.Username)) return BadRequest("Username is taken");

            var user = mapper.Map<AppUser>(register);

            user.UserName = user.UserName.ToLower();

            var result = await userManager.CreateAsync(user, "Pa$$w0rd");

            if (!result.Succeeded) return BadRequest(result.Errors);

            var role = await userManager.AddToRoleAsync(user, "Member");

            if (!role.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender,   
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.Users.
                Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null) return Unauthorized("Invalid username");

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized();

            return new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        }

        private async Task<bool> UserExist(string username)
        {
           return await userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
