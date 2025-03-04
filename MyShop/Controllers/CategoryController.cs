
using AutoMapper;
using DTO;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace MyShop.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "CategoryCache";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public CategoryController(ICategoryService categoryService, IMapper mapper, IMemoryCache cache)
        {
            _mapper = mapper;
            _categoryService = categoryService;
            _cache = cache;
        }

        //GET: api/<ProductController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get()
        {
            if (!_cache.TryGetValue(CacheKey, out IEnumerable<CategoryDTO> categoryDTOs))
            {
                IEnumerable<Category> categories = await _categoryService.GetCategoriesAsync();
                categoryDTOs = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDTO>>(categories);

                if (categoryDTOs != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = CacheDuration
                    };

                    _cache.Set(CacheKey, categoryDTOs, cacheEntryOptions);
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok(categoryDTOs);
        }
    }
}
