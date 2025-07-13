using Fiap.Hackatoon.Order.Domain.Enumerators;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Hackatoon.Order.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public static class UserList
    {
        public static IList<User>? Users { get; set; }
    }

    public class User
    {
        public int? Id { get; set; }
        public TypeRole? TypeRole { get;set;}
        public string? Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Creation { get; set; }
    }
}
