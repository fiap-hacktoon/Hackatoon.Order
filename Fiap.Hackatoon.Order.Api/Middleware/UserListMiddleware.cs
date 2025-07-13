using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Enumerators;

namespace Fiap.Hackatoon.Order.Api.Middleware
{
    public class UserListMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public Task Invoke(HttpContext httpContext)
        {
            UserList.Users = [
                                 new User { Id = 1, Email = "rmahlow@gmail.com",      Password = "admin", TypeRole = TypeRole.Manager   },
                                 new User { Id = 2, Email = "marcelocedro@gmail.com", Password = "admin", TypeRole = TypeRole.Kitchen   },
                                 new User { Id = 3, Email = "cidicley@gmail.com",     Password = "admin", TypeRole = TypeRole.Attendant },
                                 new User { Id = 3, Email = "gabriel@gmail.com",      Password = "admin", TypeRole = TypeRole.Client    },
                             ];

            return _next(httpContext);
        }
    }

    public static class UserListMiddlewareExtensions
    {
        public static IApplicationBuilder UseListaUserMiddleware(this IApplicationBuilder builder)
            => builder.UseMiddleware<UserListMiddleware>();
    }
}
