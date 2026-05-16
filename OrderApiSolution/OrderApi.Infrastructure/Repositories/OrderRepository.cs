// using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interface;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var order = context.Orders.Add(entity).Entity;
                await context.SaveChangesAsync();

                if (order.Id > 0)
                {
                    return new Response(true, "Order placed successfully");
                }
                return new Response(false, "Error while placing order");
            }
            catch (Exception ex)
            {
                // LogException.LogExceptions(ex);
                return new Response(false, "Error while placing order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);

                if (order is null)
                {
                    return new Response(false, "Order not found");
                }

                context.Orders.Remove(order);
                await context.SaveChangesAsync();
                
                return new Response(true, "Order deleted successfully");
            }
            catch (Exception ex)
            {
                // LogException.LogExceptions(ex);
                return new Response(false, "Error while placing order");
            }
        }

        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                var order = await context.Orders.FindAsync(id);

                if (order is null)
                {
                    return null!;
                }

                return order;

            }
            catch (Exception ex)
            {
                // LogException.LogExceptions(ex);
                throw new Exception("Error while finding order");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                var orders = await context.Orders.ToListAsync();

                return orders is not null ? orders : Enumerable.Empty<Order>();
            }
            catch (Exception ex)
            {
                // LogException.LogExceptions(ex);
                throw new Exception("Error while getting orders");
            }
        }

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.Where(predicate).FirstOrDefaultAsync();

                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                // LogException.LogExceptions(ex);
               throw new Exception("Error while getting order");
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var orders = await context.Orders.Where(predicate).ToListAsync();

                return orders is not null ? orders : new List<Order>();
            }
            catch (Exception ex)
            {
                // LogException.LogExceptions(ex);
                throw new Exception("Error while getting orders");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var getOrder = await FindByIdAsync(entity.Id);

                if (getOrder is null)
                {
                    return new Response(false, "Order not found");
                }

                context.Entry(getOrder).State = EntityState.Detached;
                context.Orders.Update(entity);
                await context.SaveChangesAsync();

                return new Response(true, "Order updated successfully");
            }
            catch (Exception ex)
            {
                // LogException.LogExceptions(ex);
                return new Response(false, "Error while Updating order details");
            }
        }
    }
}
