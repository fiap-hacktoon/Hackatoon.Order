using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Hackatoon.Order.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class OrderProduct : BaseEntity
    {
        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "Id da Ordem é obrigatório.")]
        public required string OrderId { get; set; }

        [Required(ErrorMessage = "Id do Produto é obrigatório.")]
        public required string ProductId { get; set; }

        [Required(ErrorMessage = "Quantidade do Produto é obrigatória.")]
        public required int Quantity { get; set; }

        [Required(ErrorMessage = "Preco do item do Pedido é obrigatória.")]
        public required decimal OrderPrice { get; set; }
    }
}
