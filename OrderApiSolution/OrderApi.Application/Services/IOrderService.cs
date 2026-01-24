using OrderApi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrderByClientId(int clientId);

        Task<OrderDetailsDto> GetOrderDetails(int orderId);

    }
}
