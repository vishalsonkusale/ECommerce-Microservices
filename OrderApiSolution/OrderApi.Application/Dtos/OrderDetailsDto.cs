using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.Dtos
{
    public record OrderDetailsDto(
        [Required] int OrderId,
        [Required] int ProductId,
        [Required] int ClientId,
        [Required] string ClientName,
        [Required, EmailAddress] string Email,
        [Required] string TelephoneNumber,
        [Required] string ProductName,
        [Required, Range(1, int.MaxValue)] int PurchaseQuantity,
        [Required, DataType(DataType.Currency)] decimal UnitPrice,
        [Required, DataType(DataType.Currency)] decimal TotalPrice,
        [Required] DateTime OrderDate
        );
}
