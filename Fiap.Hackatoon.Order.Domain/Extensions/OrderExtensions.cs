using Fiap.Hackatoon.Order.Domain.Dtos.Order;
using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Enumerators;

namespace Fiap.Hackatoon.Order.Domain.Extensions
{
    public static class OrderExtensions
    {
        public static IEnumerable<OrderDto> ToOrderDtoList(this IEnumerable<OrderEntity> orderEntities)
        {
            var ordersDto = new List<OrderDto>();

            foreach (var order in orderEntities)
                ordersDto.Add(new OrderDto
                {
                    Id = order.Id,
                    ClientId = order.ClientId,
                    OrderStatusId = (OrderStatus)order.OrderStatusId,
                    EmployeeId = order.EmployeeId,
                    Accepted = order.Accepted,
                    FinalPrice = order.FinalPrice,
                    Creation = DateTime.UtcNow.AddHours(-3),
                    Comments = order.Comments
                });

            return ordersDto;
        }

        public static OrderDto ToOrderDto(this OrderEntity orderEntity, IEnumerable<OrderProduct?> orderProducts)
        {
            var order = new OrderDto
            {
                Id = orderEntity.Id,
                ClientId = orderEntity.ClientId,
                OrderStatusId = (OrderStatus)orderEntity.OrderStatusId,
                EmployeeId = orderEntity.EmployeeId,
                Accepted = orderEntity.Accepted,
                FinalPrice = orderEntity.FinalPrice,
                Creation = orderEntity.Creation,
                LastUpdate = orderEntity.LastUpdate,
                Comments = orderEntity.Comments,
                Products = orderProducts.Where(op => op != null)
                                        .Select(op => op!.ToOrderProductDto())
                                        .ToList()
            };

            return order;
        }

        public static OrderProductDto ToOrderProductDto(this OrderProduct orderProduct)
            => new()
            {
                Id = orderProduct.Id,
                OrderId = orderProduct.OrderId,
                ProductId = orderProduct.ProductId,
                Quantity = orderProduct.Quantity,
                OrderPrice = orderProduct.OrderPrice,
                Creation = orderProduct.Creation,
                LastUpdate = orderProduct.LastUpdate
            };

        public static ProductDto ToProductDto(this Product product)
            => new()
            {
                Id = product.Id,
                TypeId = product.TypeId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Status = product.Status,
                CreatedAt = product.CreatedAt,
                Removed = product.Removed,
                RemovedAt = product.RemovedAt,
            };

        public static OrderEntity ToAddEntity(this OrderCreateDto orderDto)
                    => new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ClientId = orderDto.ClientId,
                        OrderStatusId = (int)orderDto.OrderStatusId,
                        EmployeeId = orderDto.EmployeeId,
                        Comments = orderDto.Comments,
                        Creation = orderDto.Creation,
                        LastUpdate = DateTime.UtcNow.AddHours(-3)
                    };

        public static OrderEntity ToEntity(this OrderDto orderDto)
            => new()
            {
                Id = orderDto.Id,
                ClientId = orderDto.ClientId,
                OrderStatusId = (int)orderDto.OrderStatusId,
                EmployeeId = orderDto.EmployeeId,
                Accepted = orderDto.Accepted,
                FinalPrice = orderDto.FinalPrice,
                Comments = orderDto.Comments,
                Creation = orderDto.Creation,
                LastUpdate = DateTime.UtcNow.AddHours(-3)
            };

        public static OrderDto ToOrderEvalueatedDto(this OrderDto order, bool accepted, string comments)
        {
            order.Accepted = accepted ? 1 : 0;
            order.LastUpdate = DateTime.UtcNow.AddHours(-3);
            order.Comments = comments;
            order.OrderStatusId = order.Accepted == 1 ?
                                  OrderStatus.EmPreparacao :
                                  OrderStatus.Reprovado;
            order.Evaluation = true;

            return order;
        }

        public static bool IsInvalidStatusUpdate(this OrderStatus valor)
        {
            return new[] 
            {
                OrderStatus.Entregue,
                OrderStatus.Cancelado,
                OrderStatus.EmPreparacao,
                OrderStatus.ProntoRetirada
            }
            .Contains(valor);
        }
    }
}
