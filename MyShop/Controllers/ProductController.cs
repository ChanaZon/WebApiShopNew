using Microsoft.AspNetCore.Mvc;
using Services;
using Entities.Models;
using DTO;
using AutoMapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyShop.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        IProductService _productService;
        ILogger<ProductController> _logger;
        public ProductController(IProductService productService, IMapper mapper,ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListProductDTO>>> Get([FromQuery] int? minPrice, [FromQuery] int? maxPrice, [FromQuery] int?[] categoryIds, [FromQuery] string? desc)
        {
            IEnumerable<ListProductDTO> productDTOs = await _productService.GetProductsAsync(minPrice, maxPrice, categoryIds, desc);
            if (productDTOs != null)
            {
                _logger.LogInformation("Order controller:get");
                return Ok(productDTOs);
            }
            else
            {
                _logger.LogError("Order controller:get Error");
                return BadRequest();
            }
        }
    }
}
