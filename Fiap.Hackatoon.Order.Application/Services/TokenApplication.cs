using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Enumerators;
using Fiap.Hackatoon.Order.Domain.Interfaces.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fiap.Hackatoon.Order.Application.Services
{
    public class TokenApplication(IConfiguration configuration) : ITokenApplication
    {
        private readonly IConfiguration _configuration = configuration;
        public string GenerateToken(User user)
        {
            try
            {
                int usuarioExiste = UserList.Users?.Any(u => u.Email == user.Email && u.Password == user.Password) ?? false ? 1 : 0;

                TypeRole typePermission;

                if (usuarioExiste == 0)
                    return string.Empty;
                else
                {
                    var userConvert = UserList.Users?.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                    typePermission = userConvert?.TypeRole ?? TypeRole.Manager;
                }

                var tokeHandler = new JwtSecurityTokenHandler();                
                var chaveCriptografia = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretJWT") ?? string.Empty);

                var tokenPropriedades = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty),
                        new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
                        new Claim(ClaimTypes.Role, typePermission.ToString())

                    ]),

                    //tempo de expiração do token
                    Expires = DateTime.UtcNow.AddHours(2),

                    //chave de criptografia
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chaveCriptografia),
                                                                SecurityAlgorithms.HmacSha256Signature)
                };

                //cria o token
                var token = tokeHandler.CreateToken(tokenPropriedades);

                //retorna o token criado
                return tokeHandler.WriteToken(token);                
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
    }
}
