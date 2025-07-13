using Fiap.Hackatoon.Order.Domain.Entities;

namespace Fiap.Hackatoon.Order.Domain.Interfaces.Application
{
    public interface ITokenApplication
    {
        string GenerateToken(User user);
    }
}
