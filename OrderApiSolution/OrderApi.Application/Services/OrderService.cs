using OrderApi.Application.Dtos;
using OrderApi.Application.Dtos.Conversions;
using OrderApi.Application.Interface;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services
{
    internal class OrderService(IOrder orderInterface, HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        // Get product details from Product API 
        public async Task<ProductDto> GetProduct(int productId)
        {
            // Call Product API using HttpClient with Polly resilience pipeline
            // Redirect the call to API gateway since it handles the routing to the Product API

            var getProductRequest = await httpClient.GetAsync($"/api/products/{productId}");

            if (!getProductRequest.IsSuccessStatusCode)
            {
                return null!;
            }

            var product = await getProductRequest.Content.ReadFromJsonAsync<ProductDto>();

            return product!;

        }

        // Get User details from User API
        public async Task<AppUserDto> GetUser(int userId)
        {
            // Call User API using HttpClient with Polly resilience pipeline
            // Redirect the call to API gateway since it handles the routing to the User API
            var getUserRequest = await httpClient.GetAsync($"/api/users/{userId}");
            if (!getUserRequest.IsSuccessStatusCode)
            {
                return null!;
            }
            var user = await getUserRequest.Content.ReadFromJsonAsync<AppUserDto>();
            return user!;
        }

        // Get Order Details by order Id
        public async Task<OrderDetailsDto> GetOrderDetails(int orderId)
        {
            // Prepare Order
            var order = await orderInterface.FindByIdAsync(orderId);

            if (order is null || order!.Id <= 0)
            {
                return null!;
            }

            // Get Retry pipleline
            var retryPipeline = resiliencePipeline.GetPipeline("RetryPipleline");

            // Get Product details
            var productDto = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            // Prepare Client
            var appUserDto = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            // Populate OrderDetailsDto
            return new OrderDetailsDto(
                order.Id,
                productDto.id,
                appUserDto.Id,
                appUserDto.Name,
                appUserDto.Email,
                appUserDto.TelephoneNumber,
                productDto.name,
                order.PurchaseQuantity,
                productDto.price,
                productDto.price * order.PurchaseQuantity,
                order.OrderDate
                );
        }

        // Get Orders by Client Id
        public async Task<IEnumerable<OrderDto>> GetOrderByClientId(int clientId)
        {
            // Get all Client Orders
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);

            if (!orders.Any())
            {
                return null!;
            }

            // Convert from Entity to Dto
            var (_, ordersDto) = OrderConversions.FromEntity(null, orders);

            return ordersDto!;
        }

    }
}
