using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.Dtos
{
    public record ProductDto(int id, 
        [Required]string name, 
        [Required, Range(1, int.MaxValue)] int quantity, 
        [Required, DataType(DataType.Currency)] decimal price);
}
