using AutoMapper;
using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.ServiceAbstraction;
using ECommerce.Shared.CommonResult;
using ECommerce.Shared.IdentityDtos;
using ECommerce.Shared.OrderDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Service
{
    public class AuthenticationSerivce : IAuthenticationSerivce
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configration;
        private readonly IMapper _mapper;

        public AuthenticationSerivce(UserManager<ApplicationUser> userManager, IConfiguration configration,IMapper mapper)
        {
            _userManager = userManager;
            _configration = configration;
            _mapper = mapper;
        }

        //
        public async Task<Result<IdentityAddressDto>> UpdateCurrentUserAddressAsync(IdentityAddressDto addressDto, string email)
        {
            var user = await _userManager.Users.Include(A => A.Address).FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user is null) return Error.NotFound();

            if (user.Address is null)
            {
                //Create new Address
                user.Address = _mapper.Map<Address>(addressDto);
            }
            else
            {
                //Update Old Address
                user.Address.FistName = addressDto.FistName;
                user.Address.LastName = addressDto.LastName;
                user.Address.City = addressDto.City;
                user.Address.Country = addressDto.Country;
                user.Address.Street = addressDto.Street;
            }
            await _userManager.UpdateAsync(user);

            return _mapper.Map<IdentityAddressDto>(user.Address);
        }
        //
        public async Task<Result<IdentityAddressDto>> GetCurrentUserAddressAsync(string email)
        {
            var user = await _userManager.Users.Include(A => A.Address).FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user is null) return Error.NotFound();
            return _mapper.Map<IdentityAddressDto>(user.Address);
        }
      



        public async Task<bool> CheckEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<Result<UserDto>> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return Error.NotFound();
            return new UserDto(user.Email!, user.DisplayName, await CreateTokenAsync(user));
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Error.InvalidCredintails("User InvalidCred");

            var IsPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!IsPasswordValid)
                return Error.InvalidCredintails("Password InvalidCred");
            var Token = await CreateTokenAsync(user);

            return new UserDto(user.Email!, user.DisplayName, Token);

        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto)
        {
            var User = new ApplicationUser
            {
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber
            };
            var result = await _userManager.CreateAsync(User, registerDto.Password);
            if (result.Succeeded)
            {
                var Token = await CreateTokenAsync(User);

                return new UserDto(User.Email, User.DisplayName, Token);
            }

            return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

        }
        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            // Token [Issure,Audience,Expire,SigninCreadinals,Clams]

            // Claims=> Name ,Email,Rule

            var claims = new List<Claim>()
            {
                new Claim (JwtRegisteredClaimNames.Email,user.Email!),
                new Claim (JwtRegisteredClaimNames.Name,user.UserName!),
            };
            // Roles => ask usermanger to get role 
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var secretKey = _configration["JwtOptions:secretKey"];
            //EncodingSecretKey
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var Token = new JwtSecurityToken
                (
                issuer: _configration["JwtOptions:issuer"],
                audience: _configration["JwtOptions:audience"],
                expires: DateTime.UtcNow.AddHours(1),
                claims: claims,
                signingCredentials: cred

                );
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
    }
}
