using Fiap.Hackatoon.Order.Domain.Enumerators;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fiap.Hackatoon.Order.Domain.Dtos.Order
{
    public class OrderCreateDto
    {
        [Required(ErrorMessage = "Id do Cliente é obrigatório.")]
        public required int ClientId { get; set; }

        [JsonIgnore]
        public OrderStatus OrderStatusId { get; set; } = OrderStatus.Pendente;

        public required DeliveryMode DeliveryModeId { get; set; }

        [Required(ErrorMessage = "Id do Funcionario é obrigatório.")]
        public required int EmployeeId { get; set; }

        public string Comments { get; set; }

        public DateTime Creation { get; set; }

        public List<OrderProductCreateDto> Products { get; set; } 
    }
}
