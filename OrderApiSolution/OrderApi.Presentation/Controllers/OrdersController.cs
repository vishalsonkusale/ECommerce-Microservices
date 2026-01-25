using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.Dtos;
using OrderApi.Application.Dtos.Conversions;
using OrderApi.Application.Interface;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrder order, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await order.GetAllAsync();

            if (!orders.Any())
            {
                return NotFound("No Orders Found.");
            }

            var (_, orderList) = OrderConversions.FromEntity(null, orders);

            return orderList!.Any() ? Ok(orderList) : NotFound("No Orders Found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var getOrder = await order.FindByIdAsync(id);

            if (getOrder is null)
            {
                return NotFound("Order not found.");
            }

            var (orderDto, _) = OrderConversions.FromEntity(getOrder, null);

            return Ok(orderDto);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetClientOrders(int clientId)
        {
            if (clientId <= 0)
            {
                return BadRequest("Invalid Client Id.");
            }

            var orders = await orderService.GetOrdersByClientId(clientId);
            if (!orders.Any())
            {
                return NotFound("No Orders Found for the client.");
            }
            return Ok(orders);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest("Invalid Order Id.");
            }
            var orderDetails = await orderService.GetOrderDetails(orderId);
            if (orderDetails is null || orderDetails.OrderId <= 0)
            {
                return NotFound("Order Details not found.");
            }
            return Ok(orderDetails);
        }


        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Incomple Data submitted.");
            }

            var getEntity = OrderConversions.ToEntity(orderDto);

            var response = await order.CreateAsync(getEntity);

            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Incomple Data submitted.");
            }
            var getEntity = OrderConversions.ToEntity(orderDto);
            var response = await order.UpdateAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Incomple Data submitted.");
            }
            var getEntity = OrderConversions.ToEntity(orderDto);
            var response = await order.DeleteAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }
    }
}
