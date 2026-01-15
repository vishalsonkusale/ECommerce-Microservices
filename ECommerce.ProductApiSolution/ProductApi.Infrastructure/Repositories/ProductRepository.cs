using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
    internal class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                // check if product with the same name exists
                var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                {
                    return new Response(false, $"Product with the name {entity.Name} already exists.");
                }

                // add product
                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();

                // check if product is created
                if (currentEntity is not null && currentEntity.Id > 0)
                {
                    return new Response(true, $"Product: {entity.Name} created successfully.");
                }
                else
                {
                    return new Response(false, $"Failed to create the product: {entity.Name}.");
                }

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "An error occurred while creating the product.");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                // Find the product by Id
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                {
                    return new Response(false, $"Product with Id: {entity.Id} not found");
                }

                // Remove the product
                context.Products.Remove(product);
                await context.SaveChangesAsync();

                return new Response(true, $"Product with Id: {entity.Id} deleted successfully.");   
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "An error occurred while deleting the product.");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                // Find the product by Id
                var product = await context.Products.FindAsync(id);

                return product is null ? null! : product;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("An error occurred while retrieving the product.");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();

                return products is null ? null! : products;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("An error occurred while retrieving the product.");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync();

                return product is null ? null! : product;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("An error occurred while retrieving the product.");
            }

        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                // Find the product by Id
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                {
                    return new Response(false, $"Product {entity.Name} not found.");
                }

                // Update the product details
                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();

                return new Response(true, $"Product: {entity.Name} updated successfully.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new  Response(false, $"An error occurred while updating the product: {entity.Name}.");
            }
        }
    }
}
