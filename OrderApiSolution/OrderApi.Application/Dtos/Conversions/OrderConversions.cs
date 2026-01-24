using OrderApi.Domain.Entities;

namespace OrderApi.Application.Dtos.Conversions
{
    public static class OrderConversions
    {
        public static Order ToEntity(OrderDto orderDto) => new()
        {
            Id = orderDto.id,
            ProductId = orderDto.productId,
            ClientId = orderDto.clientId,
            PurchaseQuantity = orderDto.purchaseQuantity,
            OrderDate = orderDto.orderDate
        };

        public static (OrderDto?, IEnumerable<OrderDto>?) FromEntity(Order? order, IEnumerable<Order>? orders)
        {
            // return Single
            if (order is not null || orders is null)
            {
                var singleOrder = new OrderDto(
                    order!.Id,
                    order.ProductId,
                    order.ClientId,
                    order.PurchaseQuantity,
                    order.OrderDate
                );
                return (singleOrder, null);
            }

            // return List
            if (orders is not null || order is null)
            {
                var orderDtos = orders!.Select(o => new OrderDto(
                    o.Id,
                    o.ProductId,
                    o.ClientId,
                    o.PurchaseQuantity,
                    o.OrderDate
                ));

                return (null, orderDtos);
            }

            return (null, null);
        }

    }
}
