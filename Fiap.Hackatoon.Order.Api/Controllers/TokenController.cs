using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Interfaces.Application;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Hackatoon.Order.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController(ITokenApplication tokenService) : Controller
    {
        private readonly ITokenApplication _tokenService = tokenService;

        /// <summary>
        /// Metodo para fazer o login do usuario e retornar o token para a autenticação
        /// </summary>
        /// <remarks>
        /// Exemplo de Requisição: Não é ncessário informar o ID e o Tipo da Permissão
        /// {
        /// "email": "marcelocedro@gmail.com",
        /// "password": "admin"
        /// }
        /// ou
        /// {
        /// "email": "rmahlow@gmail.com",
        /// "password": "admin"
        /// }
        /// /// ou
        /// {
        /// "email": "gabriel@gmail.com",
        /// "password": "admin"
        /// }
        /// </remarks>
        /// <param name="usuario">Realizar o Login com os usários admin,user ou guest</param>
        /// <returns>Irá retornar o token para realizar o login no Swagger</returns>
        /// <response code="200">Sucesso na execução - pode serguir com o Token</response>
        [HttpPost]
        public IActionResult GetToken([FromBody] User usuario)
        {
            var token = _tokenService.GenerateToken(usuario);

            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized();

            return Ok(token);
        }

    }
}
