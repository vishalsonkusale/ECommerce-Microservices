using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.Dtos;
using ProductApi.Application.Dtos.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController(IProduct product) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await product.GetAllAsync();

            if (!products.Any())
            {
                return NotFound("No products found.");
            }

            var (_, list) = ProductConversions.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No products found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var productEntity = await product.FindByIdAsync(id);
            if (productEntity is null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            var (productDto, _) = ProductConversions.FromEntity(productEntity, null!);

            return productDto is not null ? Ok(productDto) : NotFound($"Product with ID {id} not found.");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> CreateProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productEntity = ProductConversions.ToEntity(productDto);

            var response = await product.CreateAsync(productEntity);
            
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productEntity = ProductConversions.ToEntity(productDto);
            
            var response = await product.UpdateAsync(productEntity);

            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteProduct([FromBody] ProductDto productDto)
        {
            var productEntity = ProductConversions.ToEntity(productDto);
            
            var response = await product.DeleteAsync(productEntity);

            return response.Flag ? Ok(response) : BadRequest(response);
        }
    }
}
