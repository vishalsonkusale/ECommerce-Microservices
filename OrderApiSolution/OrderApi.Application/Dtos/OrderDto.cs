using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.Dtos
{
    public record OrderDto(int id,
        [Required, Range(1, int.MaxValue)]
        int productId,
        [Required, Range(1, int.MaxValue)]
        int clientId,
        [Required, Range(1, int.MaxValue)]
        int purchaseQuantity,
        DateTime orderDate);
}
