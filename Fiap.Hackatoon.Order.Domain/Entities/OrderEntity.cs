using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Hackatoon.Order.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class OrderEntity : BaseEntity
    {
        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "Id do Cliente é obrigatório.")]
        public required int ClientId { get; set; }

        [Required(ErrorMessage = "Situacao do Pedido é obrigatório.")]
        public required int OrderStatusId { get; set; }

        [Required(ErrorMessage = "Id do Funcionario é obrigatório.")]
        public required int EmployeeId { get; set; }

        public int? Accepted { get; set; }

        [Required(ErrorMessage = "Meio de entrega do Pedido é obrigatório.")]
        public required int DeliveryModeId { get; set; }

        [Required(ErrorMessage = "Preço final é obrigatório.")]
        public decimal FinalPrice { get; set; }

        public string Comments { get; set; }
    }
}
