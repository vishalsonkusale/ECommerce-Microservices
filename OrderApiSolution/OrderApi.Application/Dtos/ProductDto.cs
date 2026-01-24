using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.Dtos
{
    public record ProductDto(int id,
        [Required]
        string name,
        [Required, Range(1, int.MaxValue)]
        int Quantity,
        [Required, DataType(DataType.Currency)]
        decimal price);
}
