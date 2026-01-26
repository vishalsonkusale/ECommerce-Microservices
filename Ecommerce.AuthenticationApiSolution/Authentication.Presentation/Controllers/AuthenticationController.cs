using Authentication.Application.Dtos;
using Authentication.Application.Interfaces;
using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.Dtos;

namespace Authentication.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IUser user) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register(AppUserDto appUserDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await user.Register(appUserDto);

            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Register(LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await user.Login(loginDto);

            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetUserDto>> GetUser(int id)
        {
            if (id <= 0) return BadRequest();

            var result = await user.GetUser(id);

            return result.Id > 0 ? Ok(result) : NotFound();
        }
    }
}
