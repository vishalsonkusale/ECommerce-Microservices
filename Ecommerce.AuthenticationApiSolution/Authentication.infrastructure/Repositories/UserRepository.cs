using Authentication.Application.Dtos;
using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Authentication.infrastructure.Data;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderApi.Application.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration config) : IUser
    {
        private async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            return user is null ? null! : user;
        }

        public async Task<GetUserDto> GetUser(int userId)
        {
            var user = await context.Users.FindAsync(userId);

            return user is null ? null! : new GetUserDto(
                user.Id!,
                user.Name!,
                user.TelephoneNumber!,
                user.Address!,
                user.Email!,
                user.Role!  
                );
        }

        public async Task<Response> Login(LoginDto loginDto)
        {
            var getUser = await GetUserByEmail(loginDto.Email);

            if (getUser is null)
            {
                return new Response
                {
                    Flag = false,
                    Message = "User with this email does not exist"
                };
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, getUser.Password);

            if (!isPasswordValid)
            {
                return new Response
                {
                    Flag = false,
                    Message = "Invalid password"
                };
            }

            string token = GenerateToken(getUser);

            return new Response
            {
                Flag = true,
                Message = token
            };
        }

        private string GenerateToken(AppUser getUser)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);

            var securityKey = new SymmetricSecurityKey(key);

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, getUser.Name!),
                new Claim(ClaimTypes.Email, getUser.Email!),
            };

            if (!string.IsNullOrEmpty(getUser.Role))
            {
                claims = claims.Append(new Claim(ClaimTypes.Role, getUser.Role!)).ToArray();
            }

            var token = new JwtSecurityToken(
                issuer: config.GetSection("Authentication:Issuer").Value,
                audience: config.GetSection("Authentication:Audience").Value,
                claims: claims,
                expires: null,
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(AppUserDto appUserDto)
        {
            var getUser = await GetUserByEmail(appUserDto.Email);

            if (getUser is not null)
            {
                return new Response
                {
                    Flag = false,
                    Message = "User with this email already exists"
                };
            }

            var result = await context.Users.AddAsync(new AppUser
            {
                Name = appUserDto.Name,
                TelephoneNumber = appUserDto.TelephoneNumber,
                Address = appUserDto.Address,
                Email = appUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDto.Password),
                Role = appUserDto.Role
            });

            await context.SaveChangesAsync();

            return result.Entity.Id > 0 ? new Response
            {
                Flag = true,
                Message = "User registered successfully"
            } : new Response
            {
                Flag = false,
                Message = "User registration failed"
            };  
        }
    }
}
