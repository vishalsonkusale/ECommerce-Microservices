using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.Dtos
{
    public record GetUserDto(int Id,
        [Required]
        string Name,
        [Required]
        string TelephoneNumber,
        [Required]
        string Address,
        [Required, EmailAddress]
        string Email,
        [Required]
        string Role
        );
}