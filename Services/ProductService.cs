using Entities.Models;
using Microsoft.Extensions.Caching.Distributed;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using AutoMapper;
using DTO;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IDistributedCache _cache;
        IProductRepository _productRepository;
        IMapper _mapper;


        public ProductService(IDistributedCache cache, IProductRepository productRepository,IMapper mapper)
        {
            _cache = cache;
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ListProductDTO>> GetProductsAsync(int? minPrice, int? maxPrice, int?[] categoryIds, string? desc)
        {
            var filter = $"{minPrice}:{maxPrice}:{string.Join(":", categoryIds ?? [])}:{desc}";
            var cacheKey = $"products:{filter}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                return JsonSerializer.Deserialize<IEnumerable<ListProductDTO>>(cached);
            }

            var products = await _productRepository.GetProductsAsync(minPrice, maxPrice, categoryIds, desc);
            IEnumerable<ListProductDTO> productDTOs = _mapper.Map<IEnumerable<Product>, IEnumerable<ListProductDTO>>(products);
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(productDTOs));
            return productDTOs;
        }

      

        private async Task ClearProductsCacheAsync()
        {
            // אין תמיכה מובנית למחיקת כל המפתחות ב-IDistributedCache, לכן יש לשמור רשימה של מפתחות או להשתמש ב-Redis ישירות.
            // לדוגמה, אם יש לך גישה ל-ConnectionMultiplexer של Redis:
            // var redis = ConnectionMultiplexer.Connect("localhost:6379");
            // var server = redis.GetServer("localhost:6379");
            // foreach (var key in server.Keys(pattern: "products:*"))
            //     await _cache.RemoveAsync(key);

            // אם אין, אפשר לשמור רשימה של מפתחות בקאש ולמחוק אותם בלולאה.
        }
    }
}
