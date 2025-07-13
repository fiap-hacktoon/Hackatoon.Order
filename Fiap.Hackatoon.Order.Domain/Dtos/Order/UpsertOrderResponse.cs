using System.Diagnostics.CodeAnalysis;

namespace Fiap.Hackatoon.Order.Domain.Dtos.Order
{
    [ExcludeFromCodeCoverage]
    public class UpsertOrderResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
