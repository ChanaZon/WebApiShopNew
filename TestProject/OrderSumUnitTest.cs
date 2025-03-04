using AutoMapper;
using Entities.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using Repositories;
using Services;
using DTO;

namespace TestProject
{
    public class OrderSumUnitTest
    {


        [Fact]
        public async void CheckOrderSum_ValidCredentialsReturnOrder()
        {
            var products = new List<Product>
        {
            new Product { ProductId = 1, Price = 40 },
            new Product { ProductId = 2, Price = 20 }
        };

            var orders = new List<Order>
        {
            new Order
            {
                UserId = 1,
                OrderSum = 100,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2 },
                    new OrderItem { ProductId = 2, Quantity = 1 }
                }
            }
        };
            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<Order, PostOrderDTO>();
            });
            var mapper = mapperConfig.CreateMapper();
            var mockContext = new Mock<MyShopContext>();
            mockContext.Setup(x => x.Products).ReturnsDbSet(products);
            mockContext.Setup(x => x.Orders).ReturnsDbSet(orders);
            mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            var productRepository = new ProductRepository(mockContext.Object);

            var orderRepository = new OrderRepository(mockContext.Object,mapper);
            var mockLogger = new Mock<ILogger<OrderService>>();
            var orderService = new OrderService(orderRepository, productRepository, mockLogger.Object);

            var result = await orderService.AddOrderAsync(orders[0]);
            Assert.Equal(result, orders[0]);
        }

        [Fact]
        public async Task CheckOrderSum_UnValidCredentialsReturnExeption()
        {
            var products = new List<Product>
        {
            new Product { ProductId = 1, Price = 40 },
            new Product { ProductId = 2, Price = 20 }
        };

            var invalidOrder = new Order
            {
                UserId = 1,
                OrderSum = 10,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 1 },
                    new OrderItem { ProductId = 1, Quantity = 1 },
                    new OrderItem { ProductId = 2, Quantity = 1 }
                }
            };

            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<Order, PostOrderDTO>();
            });
            var mapper = mapperConfig.CreateMapper();

            var mockContext = new Mock<MyShopContext>();
            mockContext.Setup(x => x.Products).ReturnsDbSet(products);
            mockContext.Setup(x => x.Orders).ReturnsDbSet(new List<Order>());
            mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            var productRepository = new ProductRepository(mockContext.Object);
            var orderRepository = new OrderRepository(mockContext.Object, mapper);
            var mockLogger = new Mock<ILogger<OrderService>>();
            var orderService = new OrderService(orderRepository, productRepository, mockLogger.Object);

            // Act
            var result = await orderService.AddOrderAsync(invalidOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result.OrderSum);
            Assert.Equal(invalidOrder.UserId, result.UserId);
            Assert.Equal(invalidOrder.OrderItems.Count, result.OrderItems.Count);
        }
    }
}
