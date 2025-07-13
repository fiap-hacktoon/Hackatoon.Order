using Fiap.Hackatoon.Order.Domain.Entities;

namespace Fiap.Hackatoon.Order.Domain.Interfaces.Integrations
{
    public interface IProductConsultManager
    {
        Task<string> GetToken();

        Task<Product> GetProductById(string email, string token);
    }
}