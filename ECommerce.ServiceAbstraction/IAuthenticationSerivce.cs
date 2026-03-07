using ECommerce.Shared.CommonResult;
using ECommerce.Shared.IdentityDtos;
using ECommerce.Shared.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.ServiceAbstraction
{
    public interface IAuthenticationSerivce
    {
        // login =>Token ,DisplayName ,Email
        // register=> UserDto
        Task<Result<UserDto>> LoginAsync(LoginDto loginDto);
         Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto);

        Task<bool> CheckEmailAsync(string email);
        Task<Result<UserDto>> GetUserByEmailAsync(string email);

        Task<Result<IdentityAddressDto>> GetCurrentUserAddressAsync(string email);
        Task<Result<IdentityAddressDto>> UpdateCurrentUserAddressAsync(IdentityAddressDto addressDto, string email);

    }
}
