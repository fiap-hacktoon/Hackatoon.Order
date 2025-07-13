using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Hackatoon.Order.Domain.Dtos.Elastic
{
    [ExcludeFromCodeCoverage]
    public class OrderElasticDto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Id do Cliente é obrigatório.")]
        public required string ClientId { get; set; }

        [Required(ErrorMessage = "Situacao do Pedido é obrigatório.")]
        public required int OrderStatusId { get; set; }

        [Required(ErrorMessage = "Id do Funcionario é obrigatório.")]
        public required string EmployeeId { get; set; }

        public int? Accepted { get; set; }

        [Required(ErrorMessage = "Preço final é obrigatório.")]
        public required decimal FinalPrice { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RemovedAt{ get; set; }

    }
}
