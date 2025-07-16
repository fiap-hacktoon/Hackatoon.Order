using Fiap.Hackatoon.Order.Domain.Dtos.Order;
using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Enumerators;
using MassTransit.Riders;

namespace Fiap.Hackatoon.Order.Domain.Extensions
{
    public static class OrderExtensions
    {
        public static OrderDto ToOrderDto(this OrderEntity orderEntity, IEnumerable<OrderProduct?> orderProducts)
        {
            var order = new OrderDto
            {
                Id = orderEntity.Id,
                ClientId = orderEntity.ClientId,
                OrderStatusId = (OrderStatus)orderEntity.OrderStatusId,
                EmployeeId = orderEntity.EmployeeId,
                DeliveryModeId = (DeliveryMode)orderEntity.DeliveryModeId,
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

        public static OrderProductUpdateDto ToOrderProductUpdateDto(this OrderProductDto orderProduct)
            => new()
            {
                OrderId = orderProduct.OrderId,
                ProductId = orderProduct.ProductId,
                Quantity = orderProduct.Quantity,
                OrderPrice = orderProduct.OrderPrice,
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
                        DeliveryModeId = (int)orderDto.DeliveryModeId,
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
                DeliveryModeId = (int)orderDto.DeliveryModeId,
                FinalPrice = orderDto.FinalPrice,
                Comments = orderDto.Comments,
                Creation = orderDto.Creation,
                LastUpdate = DateTime.UtcNow.AddHours(-3)
            };

        public static OrderUpdateDto ToOrderEvalueatedDto(this OrderDto orderDto, bool accepted, string comments)
        {
            var orderUpdate = new OrderUpdateDto
            {
                Id = orderDto.Id,
                ClientId = orderDto.ClientId,
                OrderStatusId = accepted ?
                                OrderStatus.EmPreparacao :
                                OrderStatus.Reprovado,
                EmployeeId = orderDto.EmployeeId,
                DeliveryModeId = (DeliveryMode)orderDto.DeliveryModeId,
                Accepted = accepted ? 1 : 0,
                FinalPrice = orderDto.FinalPrice,
                Creation = orderDto.Creation,
                Comments = comments,
                Products = orderDto.Products.Where(op => op != null)
                                            .Select(op => op!.ToOrderProductUpdateDto())
                                            .ToList()
            };

            return orderUpdate;
        }

        public static OrderUpdateDto ToOrderChangeStatusdDto(this OrderDto orderDto, OrderStatus orderStatus)
        {
            var orderUpdate = new OrderUpdateDto
            {
                Id = orderDto.Id,
                ClientId = orderDto.ClientId,
                OrderStatusId = orderStatus,
                EmployeeId = orderDto.EmployeeId,
                DeliveryModeId = orderDto.DeliveryModeId,
                Accepted = orderDto.Accepted,
                FinalPrice = orderDto.FinalPrice,
                Creation = orderDto.Creation,
                Comments = orderDto.Comments,
                Products = orderDto.Products.Where(op => op != null)
                                            .Select(op => op!.ToOrderProductUpdateDto())
                                            .ToList()
            };

            return orderUpdate;
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
