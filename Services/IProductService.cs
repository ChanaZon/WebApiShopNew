using DTO;
using Entities.Models;

namespace Services
{
    public interface IProductService
    {
        Task<IEnumerable<ListProductDTO>> GetProductsAsync(int? minPrice, int? maxPrice, int?[] categoryIds, string? desc);
    }
}