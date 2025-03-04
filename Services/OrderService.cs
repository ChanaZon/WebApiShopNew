using DTO;
using Entities.Models;
using Microsoft.Extensions.Logging;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {

        IOrderRepository _orderRepository;
        IProductRepository _productRepository;
         ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, ILogger<OrderService> logger)

        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
        }
        public async Task<GetOrderDTO> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }
        public async Task<Order> AddOrderAsync(Order order)
        {
            int realSum = await CheckSum(order);
            if (realSum!=order.OrderSum)
            {
                   order.OrderSum = realSum;
                 _logger.LogWarning($"user {order.UserId} tried to change order {order.OrderId} sum");
            }
            return await _orderRepository.AddOrderAsync(order);
        }

        public async Task<int> CheckSum(Order order)
        {
            int sum = 0;
            foreach (var item in order.OrderItems)
            {
                 Product p = await _productRepository.GetProductById( item.ProductId);
                 sum += p.Price;

            }
            return sum;
        }
    }
}
