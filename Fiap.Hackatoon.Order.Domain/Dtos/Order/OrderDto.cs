using Fiap.Hackatoon.Order.Domain.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace Fiap.Hackatoon.Order.Domain.Dtos.Order
{
    public class OrderDto
    {
        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "Id do Cliente é obrigatório.")]
        public required int ClientId { get; set; }

        [Required(ErrorMessage = "Situacao do Pedido é obrigatório.")]
        public required OrderStatus OrderStatusId { get; set; }

        public required DeliveryMode DeliveryModeId { get; set; }

        [Required(ErrorMessage = "Id do Funcionario é obrigatório.")]
        public required int EmployeeId { get; set; }

        public int? Accepted { get; set; }

        [Required(ErrorMessage = "Preço final é obrigatório.")]
        public required decimal FinalPrice { get; set; }

        public string? Comments { get; set; }

        public DateTime Creation { get; set; }

        public DateTime? LastUpdate { get; set; }

        public List<OrderProductDto> Products { get; set; }
    }
}
