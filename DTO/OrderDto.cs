using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DTO
{
    public record GetOrderDTO(int OrderId,ICollection<OrderItemDTO> OrderItems,DateOnly OrderDate,int OrderSum,int UserId);
    public record PostOrderDTO(ICollection<OrderItemDTO> OrderItems, DateOnly OrderDate, int UserId,int OrderSum);
    public record OrderItemDTO(int ProductId);

}
