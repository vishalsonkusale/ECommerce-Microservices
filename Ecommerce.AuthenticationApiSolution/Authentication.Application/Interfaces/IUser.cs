using Authentication.Application.Dtos;
using ECommerce.SharedLibrary.Responses;
using OrderApi.Application.Dtos;

namespace Authentication.Application.Interfaces
{
    public interface IUser
    {
        Task<Response> Register (AppUserDto appUserDto);

        Task<Response> Login (LoginDto loginDto);

        Task<GetUserDto> GetUser (int userId);
    }
}
