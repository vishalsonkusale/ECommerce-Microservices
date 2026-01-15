using ProductApi.Domain.Entities;

namespace ProductApi.Application.Dtos.Conversions
{
    public static class ProductConversions
    {
        public static Product ToEntity(ProductDto productDto) => new()
        {
            Id = productDto.id,
            Name = productDto.name,
            Price = productDto.price,
            Quantity = productDto.quantity
        };

        public static (ProductDto?, IEnumerable<ProductDto>?) FromEntity(Product? product, IEnumerable<Product>? products)
        {
            // return single 
            if (product is not null || products is null)
            {
                var singleProduct = new ProductDto(product!.Id, product.Name!, product.Quantity, product.Price);
                return (singleProduct, null);
            }
            
            // return List
            if (product is null || products is not null)
            {
                var productDtos = products!.Select(p => new ProductDto(p.Id, p.Name!, p.Quantity, p.Price)).ToList();
                return (null, productDtos);
            }

            return (null, null);

        }
    }
}
